using FrameUp.OrderService.Application.Models.Requests;

namespace FrameUp.OrderService.Application.Contracts;

public interface IFileBucketRepository
{
    public Task Upload(FileBucketRequest request);
}