namespace FrameUp.OrderService.Domain.Entities;

public class PackageItem
{
    public string FileName { get; }
    public string Uri { get; }

    public PackageItem(string fileName, string uri)
    {
        FileName = fileName;
        Uri = uri;
    }
}
