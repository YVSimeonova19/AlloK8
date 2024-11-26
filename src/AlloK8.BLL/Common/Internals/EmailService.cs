using System;
using System.Threading.Tasks;
using AlloK8.BLL.Common.Contracts;
using AlloK8.BLL.Common.Models;
using Essentials.Results;
using Microsoft.Extensions.DependencyInjection;

namespace AlloK8.BLL.Common.Internals;

internal class EmailService : IEmailService
{
    private readonly IServiceProvider serviceProvider;
    public EmailService(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }
    public async Task<StandardResult> SendEmailAsync(EmailModel model, string senderStrategy)
    {
        var sender = this.serviceProvider.GetKeyedService<IEmailSender>(senderStrategy);
        if (sender == null)
        {
            return StandardResult
                .UnsuccessfulResult($"There is no registered email sender strategy: '{senderStrategy}'");
        }

        return await sender.SendEmailAsync(model);
    }
}
