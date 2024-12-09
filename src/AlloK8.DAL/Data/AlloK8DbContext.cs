using Microsoft.EntityFrameworkCore;
using AlloK8.DAL.Models;

namespace AlloK8.DAL.Data;

public class AlloK8DbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Task> Tasks { get; set; }
    public DbSet<Collaboration> Collaborations { get; set; }
    public DbSet<Message> Messages { get; set; }

    public AlloK8DbContext(DbContextOptions<AlloK8DbContext> options)
        : base(options)
    {
    }
}