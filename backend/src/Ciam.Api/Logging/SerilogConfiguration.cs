using Serilog;
using Serilog.Events;

namespace Ciam.Api.Logging;

public static class SerilogConfiguration
{
    private const string LogFolderName = "logs";

    public static LoggerConfiguration ConfigureApiLogging(
        this LoggerConfiguration loggerConfiguration,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        var logDirectory = Path.GetFullPath(
            Path.Combine(environment.ContentRootPath, LogFolderName));
        Directory.CreateDirectory(logDirectory);

        return loggerConfiguration
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext()
            .Enrich.WithEnvironmentName()
            .Enrich.WithThreadId()
            .Enrich.WithProperty("Application", "Ciam.Api")
            .WriteTo.Console()
            .WriteTo.Logger(fileLogger => fileLogger
                .Filter.ByIncludingOnly(logEvent => logEvent.Level >= LogEventLevel.Error)
                .WriteTo.File(
                    path: Path.Combine(logDirectory, "critical-.log"),
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 30,
                    shared: true,
                    flushToDiskInterval: TimeSpan.FromSeconds(1)));
    }
}
