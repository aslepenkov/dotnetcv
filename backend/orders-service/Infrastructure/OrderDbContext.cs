using Microsoft.EntityFrameworkCore;
using OrdersService.Domain;

namespace OrdersService.Infrastructure;

public class OrderDbContext : DbContext
{
    public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options)
    {
    }

    public DbSet<Order> Orders => Set<Order>();
    public DbSet<Outbox> OutboxMessages => Set<Outbox>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Total).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Status).IsRequired();
        });

        modelBuilder.Entity<Outbox>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Type).IsRequired();
            entity.Property(e => e.Data).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
        });
    }
}