using Microsoft.EntityFrameworkCore;
using Server.Models;

namespace Models;
public class PubContext : DbContext
{
    public PubContext(DbContextOptions<PubContext> options) : base(options)
    {
    }

    public DbSet<Table> Tables { get; set; }
    public DbSet<Guest> Guests { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<MenuItem> MenuItems { get; set; }
    public DbSet<Waiter> Waiters { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Define composite keys, relationships, and any custom configurations here

        // Example: Configuring a Many-to-Many relationship for OrderDetails

        // You can also seed data here if necessary
    }
}