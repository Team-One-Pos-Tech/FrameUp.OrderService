namespace FrameUp.OrderService.Domain.Contracts;

public interface ILocalStoreRepository
{
    Task SaveFileAsync(Guid orderId, string fileName, Stream contentStream);
    Task<Stream> GetFileAsync(Guid orderId, string fileName);
    Task DeleteFileAsync(Guid orderId, string fileName);
}
