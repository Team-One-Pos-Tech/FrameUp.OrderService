namespace FrameUp.OrderService.Application.Repositories;

public interface IFileBucket
{
    public void Save(Stream stream, string fileName);
}