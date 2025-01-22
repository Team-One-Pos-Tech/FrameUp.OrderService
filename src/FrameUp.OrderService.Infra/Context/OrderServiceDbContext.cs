using FrameUp.OrderService.Infra.DatabaseMaps;
using Microsoft.EntityFrameworkCore;

namespace FrameUp.OrderService.Infra.Context;

public class OrderServiceDbContext : DbContext
{
    public OrderServiceDbContext(DbContextOptions<OrderServiceDbContext> dbContextOptions)
        : base(dbContextOptions)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrderServiceDbContext).Assembly);

        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new OrderMap());
    }
}