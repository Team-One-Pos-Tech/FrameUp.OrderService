namespace FrameUp.OrderService.Application.Repositories;

public interface IFileBucket
{
    public Task Save(Stream stream, string fileName);
}