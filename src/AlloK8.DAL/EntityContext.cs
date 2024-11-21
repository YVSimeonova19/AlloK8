using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AlloK8.DAL;

public class EntityContext : IdentityDbContext
{
    public EntityContext(DbContextOptions<EntityContext> options) : base(options)
    {
        
    }
}