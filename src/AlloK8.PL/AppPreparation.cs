using System;
using System.Threading.Tasks;
using AlloK8.BLL.Common.Users;
using AlloK8.BLL.Identity.Constants;
using AlloK8.DAL;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AlloK8.PL;

public static class AppPreparation
{
    public static async Task PrepareAsync(this IApplicationBuilder app)
    {
        try
        {
            using var scope = app.ApplicationServices.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<EntityContext>();
            var userService = scope.ServiceProvider.GetRequiredService<IUserService>();

            await dbContext.Database.MigrateAsync();

            if (!await dbContext.Users.AnyAsync())
            {
                using var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                var user = new ApplicationUser
                {
                    UserName = InitialAdminCredentials.AdminEmail,
                    Email = InitialAdminCredentials.AdminEmail,
                    EmailConfirmed = true,
                };
                var adminCreatedResult = await userManager.CreateAsync(user, InitialAdminCredentials.AdminPassword);
                await userService.CreateUserProfile(user.Id);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}
