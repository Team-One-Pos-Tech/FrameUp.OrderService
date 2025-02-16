using FrameUp.OrderService.Domain.Entities;
using FrameUp.OrderService.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FrameUp.OrderService.Infra.DatabaseMaps;

public class WorkbenchMap : IEntityTypeConfiguration<Workbench>
{
    public void Configure(EntityTypeBuilder<Workbench> builder)
    {
        builder
            .ToTable("Workbench")
            .HasKey(px => px.Id);

        builder
            .Property(px => px.Id)
            .HasColumnName("Id")
            .IsRequired();

        builder
            .Property(px => px.Status)
            .HasColumnName("Status")
            .HasConversion<StringToEnumConverter<WorkbenchStatus>>()
            .IsRequired();

        builder
            .Property(px => px.OrderId)
            .HasColumnName("OrderId")
            .IsRequired();

        builder
            .Property(px => px.Location)
            .HasColumnName("Location")
            .IsRequired();
        
        builder
            .Property(px => px.Error)
            .HasColumnName("Error")
            .IsRequired(false);
    }
}