using AlloK8.BLL.Common.Constants;
using AlloK8.BLL.Common.Contracts;
using AlloK8.BLL.Common.Internals;
using AlloK8.BLL.Common.Internals.EmailSenders;
using AlloK8.BLL.Common.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AlloK8.BLL.Common;

public static class DependencyInjection
{
    public static IServiceCollection AddCommonServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        //ERRORS

        //if (configuration.GetSection("Emails:Smtp").GetValue<bool>("Enabled"))
        //{
        //    services.Configure<EmailSmtpOptions>(configuration.GetSection("Emails:Smtp"));
        //    services.AddKeyedScoped<IEmailSender, SmtpSender>(EmailSenderStrategies.Smtp);
        //}

        //if (configuration.GetSection("Emails:SendGrid").GetValue<bool>("Enabled"))
        //{
        //    services.Configure<EmailSendGridOptions>(configuration.GetSection("Emails:SendGrid"));
        //    services.AddKeyedScoped<IEmailSender, SendGridSender>(EmailSenderStrategies.SendGrid);
        //}

        services.AddScoped<IEmailService, EmailService>();
        services.AddKeyedScoped<IEmailSender, NoOpsSender>(EmailSenderStrategies.NoOps);

        return services;
    }
}
