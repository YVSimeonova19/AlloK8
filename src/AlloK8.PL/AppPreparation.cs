using System;
using System.Threading.Tasks;
using AlloK8.DAL;
using Microsoft.AspNetCore.Builder;
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

            await dbContext.Database.MigrateAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}
