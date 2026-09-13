using System.Diagnostics;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using Ciam.Api;
using Ciam.Api.Logging;
using Ciam.Application;
using Ciam.Application.Abstractions.Services;
using Ciam.Application.Common.Exceptions;
using Ciam.Application.Features.Authentication.Commands;
using Ciam.Application.Features.Users.Commands;
using Ciam.Application.Features.Users.Queries;
using Ciam.Contracts.Common;
using Ciam.Contracts.Requests.Auth;
using Ciam.Contracts.Requests.Users;
using Ciam.Infrastructure;
using Ciam.Infrastructure.Identity;
using Ciam.Infrastructure.Persistence;
using Ciam.Infrastructure.Telemetry;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfiguration) =>
    loggerConfiguration.ConfigureApiLogging(
        context.Configuration,
        context.HostingEnvironment));

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, HttpContextCurrentUser>();
builder.Services.AddCors(options =>
{
    var origins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
    options.AddPolicy("frontend", policy =>
    {
        if (origins.Length == 0)
        {
            policy.SetIsOriginAllowed(_ => false);
            return;
        }

        policy.WithOrigins(origins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("auth", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 20,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            }));
});

var keycloak = builder.Configuration.GetSection(KeycloakOptions.SectionName).Get<KeycloakOptions>()
    ?? throw new InvalidOperationException("Keycloak configuration is required.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = $"{keycloak.BaseUrl.TrimEnd('/')}/realms/{Uri.EscapeDataString(keycloak.Realm)}";
        options.Audience = keycloak.ClientId;
        options.RequireHttpsMetadata = builder.Environment.IsProduction();
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            NameClaimType = "preferred_username",
            RoleClaimType = "roles",
            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("customer", policy => policy.RequireAuthenticatedUser());

builder.Services.AddOpenApi();

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService("Ciam.Api"))
    .WithTracing(tracing => tracing
        .AddSource(CiamActivitySource.Name)
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddOtlpExporter(options =>
        {
            var endpoint = builder.Configuration["OpenTelemetry:Otlp:Endpoint"];
            if (Uri.TryCreate(endpoint, UriKind.Absolute, out var otlpEndpoint))
            {
                options.Endpoint = otlpEndpoint;
            }
        }))
    .WithMetrics(metrics => metrics
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddRuntimeInstrumentation()
        .AddOtlpExporter());

var connectionString = builder.Configuration.GetConnectionString("Ciam")
    ?? throw new InvalidOperationException("ConnectionStrings:Ciam is required.");

builder.Services.AddHealthChecks()
    .AddNpgSql(connectionString, name: "postgresql", tags: ["ready", "db"])
    .AddUrlGroup(
        new Uri($"{keycloak.BaseUrl.TrimEnd('/')}/realms/{Uri.EscapeDataString(keycloak.Realm)}"),
        name: "keycloak",
        tags: ["ready", "identity"]);

builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

var app = builder.Build();

if (builder.Configuration.GetValue<bool>("Database:ApplyMigrations"))
{
    await using var scope = app.Services.CreateAsyncScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<CiamDbContext>();
    await dbContext.Database.MigrateAsync();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseSerilogRequestLogging();
app.UseForwardedHeaders();
app.UseCors("frontend");
app.UseHttpsRedirection();
app.UseHsts();
app.Use(async (context, next) =>
{
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["X-Frame-Options"] = "DENY";
    context.Response.Headers["Referrer-Policy"] = "no-referrer";
    await next();
});
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options => options
        .WithTitle("CIAM API")
        .WithTheme(ScalarTheme.DeepSpace));
}

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => false
});
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = registration => registration.Tags.Contains("ready")
});

var auth = app.MapGroup("/api/auth").WithTags("Authentication");
auth.MapPost("/register", async (
    RegisterUserRequest request,
    ISender sender,
    CancellationToken cancellationToken) =>
    Results.Ok(await sender.Send(new RegisterUserCommand(request), cancellationToken)))
    .AllowAnonymous()
    .RequireRateLimiting("auth")
    .WithName("RegisterUser");
auth.MapPost("/login", async (
    LoginRequest request,
    ISender sender,
    CancellationToken cancellationToken) =>
    Results.Ok(await sender.Send(new LoginCommand(request), cancellationToken)))
    .AllowAnonymous()
    .RequireRateLimiting("auth")
    .WithName("Login");
auth.MapPost("/refresh", async (
    RefreshTokenRequest request,
    ISender sender,
    CancellationToken cancellationToken) =>
    Results.Ok(await sender.Send(new RefreshTokenCommand(request), cancellationToken)))
    .AllowAnonymous()
    .RequireRateLimiting("auth")
    .WithName("RefreshToken");
auth.MapPost("/verify-email", async (
    VerifyEmailRequest request,
    ISender sender,
    CancellationToken cancellationToken) =>
    {
        await sender.Send(new VerifyEmailCommand(request), cancellationToken);
        return Results.NoContent();
    })
    .AllowAnonymous()
    .RequireRateLimiting("auth")
    .WithName("VerifyEmail");

var users = app.MapGroup("/api/users")
    .WithTags("Users")
    .RequireAuthorization("customer");
users.MapGet("/me", async (
    ISender sender,
    CancellationToken cancellationToken) =>
    Results.Ok(await sender.Send(new GetCurrentUserQuery(), cancellationToken)))
    .WithName("GetCurrentUser");
users.MapPut("/me", async (
    UpdateProfileRequest request,
    ISender sender,
    CancellationToken cancellationToken) =>
    Results.Ok(await sender.Send(new UpdateCurrentUserCommand(request), cancellationToken)))
    .WithName("UpdateCurrentUser");

app.Run();

public partial class Program;
