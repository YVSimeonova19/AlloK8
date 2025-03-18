using AlloK8.BLL.Common.EmailSending;
using AlloK8.BLL.Common.Projects;
using AlloK8.BLL.Common.Search;
using AlloK8.BLL.Common.Tasks;
using AlloK8.BLL.Common.Users;
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
        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ISearchService, SearchService>();

        return services;
    }
}
