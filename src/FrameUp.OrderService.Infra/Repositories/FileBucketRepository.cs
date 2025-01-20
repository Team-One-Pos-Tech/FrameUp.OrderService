using FrameUp.OrderService.Application.Models;
using FrameUp.OrderService.Application.Repositories;

namespace FrameUp.OrderService.Infra.Repositories;

public class FileBucketRepository : IFileBucketRepository
{
    public Task Save(Stream stream, VideoMetadataRequest metadata)
    {
        throw new NotImplementedException();
    }

    public Task Upload(FileBucketRequest request)
    {
        Console.WriteLine("Saving files to bucket");

        return Task.FromResult("");
    }
}