using System;
using System.Collections.Generic;
using AlloK8.BLL.Common.Users;
using AlloK8.BLL.Identity.Constants;
using AlloK8.DAL;
using AlloK8.DAL.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Task = System.Threading.Tasks.Task;

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

                await userManager.CreateAsync(user, InitialAdminCredentials.AdminPassword);
                await userService.CreateUserProfile(user.Id);
            }

            if (!await dbContext.Columns.AnyAsync())
            {
                var columns = new List<Column>
                {
                    new Column
                    {
                        Name = "todo",
                        Position = 1,
                    },
                    new Column
                    {
                        Name = "doing",
                        Position = 2,
                    },
                    new Column
                    {
                        Name = "done",
                        Position = 3,
                    },
                };

                foreach (var column in columns)
                {
                    await dbContext.Columns.AddAsync(column);
                }

                await dbContext.SaveChangesAsync();
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}
