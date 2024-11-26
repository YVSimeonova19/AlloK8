using AlloK8.BLL.Common;
using AlloK8.BLL.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AlloK8.BLL;

public static class DependencyInjection
{
    public static IServiceCollection AddServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddCommonServices(configuration);
        services.AddIdentityServices();

        return services;
    }
}