using Microsoft.EntityFrameworkCore;

namespace AsyncRestNew.Models;

public class WarehouseDbContext : DbContext
{
    public WarehouseDbContext(DbContextOptions<WarehouseDbContext> options) : base(options)
    {
    }
    
    public DbSet<Product> Product { get; set; }
    public DbSet<Warehouse> Warehouse { get; set; }
    public DbSet<Order> Order { get; set; }
    public DbSet<ProductWarehouse> Product_Warehouse { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>()
            .Property(p => p.Price)
            .HasPrecision(25, 2);

        modelBuilder.Entity<ProductWarehouse>()
            .Property(pw => pw.Price)
            .HasPrecision(25, 2);
    }
}