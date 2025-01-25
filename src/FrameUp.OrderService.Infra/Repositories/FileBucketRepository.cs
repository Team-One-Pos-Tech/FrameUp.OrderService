using FrameUp.OrderService.Application.Contracts;
using FrameUp.OrderService.Application.Models.Requests;

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