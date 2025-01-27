using FrameUp.OrderService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FrameUp.OrderService.Infra.DatabaseMaps;

internal class OrderMap : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder
            .Property(order => order.Id)
            .IsRequired();

        builder
            .Property(order => order.Status)
            .IsRequired();

        builder
            .Property(order => order.OwnerId)
            .IsRequired();

        builder
            .Property(order => order.PackageUri);

        builder
            .HasMany(order => order.Videos)
            .WithOne()
            .HasForeignKey("OrderId")
            .IsRequired();
    }
}