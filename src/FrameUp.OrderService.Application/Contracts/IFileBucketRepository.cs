using FrameUp.OrderService.Application.Models;

namespace FrameUp.OrderService.Application.Contracts;

public record FileBucketRequest
{
    public Guid OrderId { get; set; }

    public IEnumerable<FileRequest> Files { get; set; } = new List<FileRequest>();
}

public record FileRequest
{
    public Stream ContentStream { get; init; }

    public string Name { get; set; }

    public long Size { get; set; }

    public string ContentType { get; set; }
}

public interface IFileBucketRepository
{
    public Task Save(Stream stream, VideoMetadataRequest metadata);

    public Task Upload(FileBucketRequest request);
}