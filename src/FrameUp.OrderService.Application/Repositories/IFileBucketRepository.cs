using FrameUp.OrderService.Application.Models;

namespace FrameUp.OrderService.Application.Repositories;

public interface IFileBucketRepository
{
    public Task Save(Stream stream, VideoMetadataRequest metadata);
}