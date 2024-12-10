using AlloK8.DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace AlloK8.DAL.Data;

public class AlloK8DbContext : DbContext
{
    public AlloK8DbContext(DbContextOptions<AlloK8DbContext> options)
        : base(options)
    {
    }

    public DbSet<UserProfile> Users { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Task> Tasks { get; set; }
    public DbSet<Collaboration> Collaborations { get; set; }
    public DbSet<Message> Messages { get; set; }
}