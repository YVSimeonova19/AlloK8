using System;
using AlloK8.DAL.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AlloK8.DAL;

public class EntityContext
    : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    public EntityContext(DbContextOptions<EntityContext> options)
        : base(options)
    {
    }

    public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    public DbSet<UserProfile> UserProfiles { get; set; }
    public DbSet<Task> Tasks { get; set; }
    public DbSet<Column> Columns { get; set; }
    public DbSet<Project> Projects { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // USER
        // UserProfile(1) - ApplicationUser(1)
        builder
            .Entity<UserProfile>()
            .HasOne(up => up.ApplicationUser)
            .WithOne(au => au.UserProfile)
            .HasForeignKey<UserProfile>(up => up.ApplicationUserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        // TASK
        // Task(m) - UserProfile(m)
        builder
            .Entity<Task>()
            .HasMany(t => t.Assignees)
            .WithMany(u => u.Tasks);

        // Task(m) - Column(1)
        builder
            .Entity<Task>()
            .HasOne(t => t.Column)
            .WithMany(c => c.Tasks)
            .HasForeignKey(t => t.ColumnId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        //Task(m) - Project(1)
        builder
            .Entity<Task>()
            .HasOne(t => t.Project)
            .WithMany(p => p.Tasks)
            .HasForeignKey(t => t.ProjectId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        // Task(1) - Creator(m)
        builder
            .Entity<Task>()
            .HasOne(t => t.CreatedByUser)
            .WithMany(u => u.TasksCreated)
            .HasForeignKey(t => t.CreatedByUserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        // Task(1) - Updator(1)
        builder
            .Entity<Task>()
            .HasOne(t => t.UpdatedByUser)
            .WithMany(u => u.TasksUpdated)
            .HasForeignKey(t => t.UpdatedByUserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        // PROJECT
        // Project(m) - UserProfile(m)
        builder
            .Entity<Project>()
            .HasMany(p => p.Users)
            .WithMany(u => u.Projects);

        // Project(m) - Creator(1)
        builder
            .Entity<Project>()
            .HasOne(p => p.CreatedByUser)
            .WithMany(u => u.ProjectsCreated)
            .HasForeignKey(p => p.CreatedByUserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        // Project(m) - Updator(1)
        builder
            .Entity<Project>()
            .HasOne(p => p.UpdatedByUser)
            .WithMany(u => u.ProjectsUpdated)
            .HasForeignKey(p => p.UpdatedByUserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        base.OnModelCreating(builder);
    }
}