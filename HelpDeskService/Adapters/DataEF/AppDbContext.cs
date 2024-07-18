using DataEF.Configurations;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataEF;
public class AppDbContext : DbContext
{
    public DbSet<Client> Clients { get; set; }
    public DbSet<Support> Supports { get; set; }
    public DbSet<ClientSupport> ClientSupports { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    public AppDbContext(DbContextOptions options) : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new ClientsConfig());
        modelBuilder.ApplyConfiguration(new SupportConfig());
        modelBuilder.ApplyConfiguration(new ClientSupportConfig());
        modelBuilder.ApplyConfiguration(new CommentConfig());
        modelBuilder.ApplyConfiguration(new TicketConfig());
    }

}
