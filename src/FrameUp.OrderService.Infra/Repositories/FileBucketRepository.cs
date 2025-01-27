using FrameUp.OrderService.Application.Contracts;
using FrameUp.OrderService.Application.Models.Requests;
using Minio;
using Minio.DataModel.Args;
using Minio.DataModel.Tags;

namespace FrameUp.OrderService.Infra.Repositories;

public class FileBucketRepository(IMinioClient minioClient) : IFileBucketRepository
{
    private const string BucketName = "frameup.videos";

    public async Task Upload(FileBucketRequest request)
    {
        await CreateBucketIfNotExistsAsync();

        var tagging = CreateTagging(request.OrderId.ToString());

        var uploadTasks = request.Files
            .Select(file => UploadFile(request.OrderId, file, tagging));

        try
        {
            await Task.WhenAll(uploadTasks);
        }
        catch (Exception e)
        {
            throw new Exception("Error uploading files", e);
        }
    }

    private async Task UploadFile(Guid orderId, FileRequest file, Tagging tagging)
    {
        var objectName = orderId.ToString() + "/" + file.Name;

        var args = new PutObjectArgs()
            .WithBucket(BucketName)
            .WithTagging(tagging)
            .WithObject(objectName)
            .WithStreamData(file.ContentStream)
            .WithObjectSize(file.ContentStream.Length)
            .WithContentType(file.ContentType);

        await minioClient.PutObjectAsync(args);
    }

    private static Tagging CreateTagging(string orderId)
    {
        return new Tagging(new Dictionary<string, string>()
            {
                {
                    "orderId", orderId
                }
            },
            false);
    }

    private async Task CreateBucketIfNotExistsAsync()
    {
        if (await minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(BucketName)))
            return;

        await minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(BucketName));
    }

}