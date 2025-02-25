using System;
using AlloK8.DAL;
using Microsoft.EntityFrameworkCore;

namespace AlloK8.BLL.Tests;

public static class TestHelpers
{
    public static EntityContext CreateDbContext()
    {
        var dbContextOptions = new DbContextOptionsBuilder<EntityContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString());
        
        return new EntityContext(dbContextOptions.Options);
    }
}