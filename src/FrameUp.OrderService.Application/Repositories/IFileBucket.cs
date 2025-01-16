using FrameUp.OrderService.Application.Models;

namespace FrameUp.OrderService.Application.Repositories;

public interface IFileBucket
{
    public Task Save(Stream stream, VideoMetadataRequest metadata);
}