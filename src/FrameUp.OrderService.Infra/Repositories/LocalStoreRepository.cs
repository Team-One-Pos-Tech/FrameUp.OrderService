using FrameUp.OrderService.Domain.Contracts;

namespace FrameUp.OrderService.Infra.Repositories;


public class LocalStoreRepository : ILocalStoreRepository
{
    private const string BasePath = "C:\\FrameUp\\LocalStore";

    public async Task SaveFileAsync(Guid orderId, string fileName, Stream contentStream)
    {
        var path = Path.Combine(BasePath, orderId.ToString(), fileName);

        Directory.CreateDirectory(Path.GetDirectoryName(path)!);

        using (var fileStream = File.Create(path))
        {
            await contentStream.CopyToAsync(fileStream);
        }
    }

    public Stream GetFile(Guid orderId, string fileName)
    {
        var path = Path.Combine(BasePath, orderId.ToString(), fileName);

        if (!File.Exists(path))
            throw new FileNotFoundException("File not found", path);

        return File.OpenRead(path);
    }

    public Task DeleteFileAsync(Guid orderId, string fileName)
    {
        var path = Path.Combine(BasePath, orderId.ToString(), fileName);

        if (!File.Exists(path))
            throw new FileNotFoundException("File not found", path);

        File.Delete(path);

        return Task.CompletedTask;
    }
}
