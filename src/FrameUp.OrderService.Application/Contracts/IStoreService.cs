namespace FrameUp.OrderService.Application.Contracts;

public interface IStoreService
{
    public Task Save(Guid orderId, Stream[] files);
}

public class StoreService : IStoreService
{
    public async Task Save(Guid orderId, Stream[] files)
    {
        string directoryPath = Path.Combine("Orders", orderId.ToString());
        Directory.CreateDirectory(directoryPath);

        for (int i = 0; i < files.Length; i++)
        {
            string filePath = Path.Combine(directoryPath, $"file_{i}.dat");
            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                await files[i].CopyToAsync(fileStream);
            }
        }
    }
}
