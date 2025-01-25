using FrameUp.OrderService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FrameUp.OrderService.Infra.DatabaseMaps;

internal class VideoMetadataMap : IEntityTypeConfiguration<VideoMetadata>
{
    public void Configure(EntityTypeBuilder<VideoMetadata> builder)
    {
        builder
            .Property(video => video.Id)
            .ValueGeneratedOnAdd()
            .IsRequired();

        builder
            .Property(video => video.OrderId)
            .IsRequired();

        builder
            .Property(video => video.Name);

        builder
            .Property(video => video.ContentType);

        builder
            .Property(video => video.Size);
    }
}