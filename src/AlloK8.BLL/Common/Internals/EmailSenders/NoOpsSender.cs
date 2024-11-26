using System.Threading.Tasks;
using AlloK8.BLL.Common.Contracts;
using AlloK8.BLL.Common.Models;
using Essentials.Results;
using Microsoft.Extensions.Logging;

namespace AlloK8.BLL.Common.Internals.EmailSenders;

internal class NoOpsSender : IEmailSender
{
    private readonly ILogger<NoOpsSender> logger;
    public NoOpsSender(ILogger<NoOpsSender> logger)
    {
        this.logger = logger;
    }

    public Task<StandardResult> SendEmailAsync(EmailModel model)
    {
        this.logger.LogDebug("Sending email via NoOps");
        this.logger.LogDebug($"Recipient: {model.Email}");
        this.logger.LogDebug($"Subject: {model.Subject}");
        this.logger.LogDebug($"Message: {model.Message}");

        return Task.FromResult(StandardResult.SuccessfulResult());
    }
}
