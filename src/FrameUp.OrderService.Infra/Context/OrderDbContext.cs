using FrameUp.OrderService.Infra.DatabaseMaps;
using Microsoft.EntityFrameworkCore;

namespace FrameUp.OrderService.Infra.Context;

public class OrderDbContext : DbContext
{
    public OrderDbContext(DbContextOptions<OrderDbContext> dbContextOptions)
        : base(dbContextOptions)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrderDbContext).Assembly);
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new OrderMap());
    }
}