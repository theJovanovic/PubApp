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
    public DbSet<OrderDetail> OrderDetails { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Define composite keys, relationships, and any custom configurations here

        // Example: Configuring a Many-to-Many relationship for OrderDetails
        modelBuilder.Entity<OrderDetail>()
            .HasKey(od => new { od.OrderID, od.MenuItemID });

        modelBuilder.Entity<OrderDetail>()
            .HasOne(od => od.Order)
            .WithMany(o => o.OrderDetails)
            .HasForeignKey(od => od.OrderID);

        modelBuilder.Entity<OrderDetail>()
            .HasOne(od => od.MenuItem)
            .WithMany(mi => mi.OrderDetails)
            .HasForeignKey(od => od.MenuItemID);

        // You can also seed data here if necessary
    }
}