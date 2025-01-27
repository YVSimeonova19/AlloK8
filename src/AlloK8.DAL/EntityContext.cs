using System;
using AlloK8.DAL.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AlloK8.DAL;

public class EntityContext
    : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    public EntityContext(DbContextOptions<EntityContext> options)
        : base(options)
    {
    }

    public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    public DbSet<ApplicationRole> ApplicationRoles { get; set; }
    public DbSet<UserProfile> UserProfiles { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Task> Tasks { get; set; }
    public DbSet<Column> Columns { get; set; }
    public DbSet<Board> Boards { get; set; }
    public DbSet<Project> Projects { get; set; }
}