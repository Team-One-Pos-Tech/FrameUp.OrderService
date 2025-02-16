using FrameUp.OrderService.Domain.Enums;

namespace FrameUp.OrderService.Domain.Entities;

public class Workbench
{
    private const string FrameUpTempSpace = "frame_up";
    
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public string Location { get; set; } = string.Empty;
    public WorkbenchStatus Status { get; set; }
    public string Error { get; set; } = string.Empty;

    public static Workbench InitializeFor(Guid orderId)
    {
        var workbenchId = Guid.NewGuid();
        
        var tempPath = Path.GetTempPath();
        var location = Path.Combine(tempPath, FrameUpTempSpace, workbenchId.ToString());
        
        Directory.CreateDirectory(location);
        
        return new Workbench
        {
            Id = workbenchId,
            OrderId = orderId,
            Location = location,
            Status = WorkbenchStatus.Waiting
        };
    }

    public IEnumerable<FileInfo> ListAllFileDetails()
    {
        var directoryInfo = new DirectoryInfo(Location);
        return directoryInfo.GetFiles();
    }
    
    public async Task StoreStreamAsFileAsync(string fileName, Stream stream)
    {
        await using var fileStream = new FileStream(fileName, FileMode.Create);
        await stream.CopyToAsync(fileStream);
        await fileStream.FlushAsync();
    }
}