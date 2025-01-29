using AlloK8.BLL.Common.EmailSending;
using AlloK8.BLL.Common.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AlloK8.BLL.Common;

public static class DependencyInjection
{
    public static IServiceCollection AddCommonServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<EmailSendGridOptions>(configuration.GetSection("SendGrid").Bind);
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<ITaskService, TaskService>();

        return services;
    }
}
