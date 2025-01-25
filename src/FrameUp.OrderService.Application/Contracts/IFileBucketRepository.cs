using FrameUp.OrderService.Application.Models;

namespace FrameUp.OrderService.Application.Contracts;

public interface IFileBucketRepository
{
    public Task Upload(FileBucketRequest request);
}