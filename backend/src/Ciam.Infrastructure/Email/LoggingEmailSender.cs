using Ciam.Application.Abstractions.Services;
using Microsoft.Extensions.Logging;

namespace Ciam.Infrastructure.Email;

public sealed class LoggingEmailSender(ILogger<LoggingEmailSender> logger) : IEmailSender
{
    public Task SendAsync(string recipient, string subject, string body, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(recipient);
        ArgumentException.ThrowIfNullOrWhiteSpace(subject);
        ArgumentException.ThrowIfNullOrWhiteSpace(body);
        logger.LogInformation("Email delivery is not configured. Recipient: {Recipient}, Subject: {Subject}", recipient, subject);
        return Task.CompletedTask;
    }
}
