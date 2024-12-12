using AlloK8.DAL;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace AlloK8.BLL.Identity;

internal static class DependencyInjection
{
    public static IServiceCollection AddIdentityServices(
        this IServiceCollection services)
    {
         services
             .AddIdentity<ApplicationUser, ApplicationRole>(options =>
             {
                 options.SignIn.RequireConfirmedAccount = false;
                 //make the next one true later
                 options.SignIn.RequireConfirmedEmail = false;
                 options.Password.RequiredLength = 6;
             })
             .AddEntityFrameworkStores<EntityContext>()
             .AddDefaultTokenProviders();

         return services;
    }
}
