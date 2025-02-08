namespace FrameUp.OrderService.Domain.Entities;

public class PackageItem
{
    public Guid Id { get; set; }
    public string FileName { get; set; }
    public string Uri { get; set; }

    public PackageItem(string fileName, string uri)
    {
        FileName = fileName;
        Uri = uri;
    }
}
