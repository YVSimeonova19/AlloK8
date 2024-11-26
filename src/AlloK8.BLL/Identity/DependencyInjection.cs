using AlloK8.DAL;
using Microsoft.Extensions.DependencyInjection;

namespace AlloK8.BLL.Identity;

internal static class DependencyInjection
{
    public static IServiceCollection AddIdentityServices(
        this IServiceCollection services)
    {
        //ERROR

        //services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
        //{
        //    options.SignIn.RequireConfirmedAccount = true;
        //})
        //    .AddEntityFrameworkStores<EntityContext>();

        return services;
    }
}
