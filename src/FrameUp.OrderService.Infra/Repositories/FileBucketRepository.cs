using FrameUp.OrderService.Application.Contracts;
using FrameUp.OrderService.Application.Models;
using Minio;
using Minio.DataModel.Args;
using Minio.DataModel.Tags;

namespace FrameUp.OrderService.Infra.Repositories;

public class FileBucketRepository(IMinioClient minioClient) : IFileBucketRepository
{
    private const string bucketName = "frameup.videos";

    public Task Save(Stream stream, VideoMetadataRequest metadata)
    {
        throw new NotImplementedException();
    }

    public async Task Upload(FileBucketRequest request)
    {
        await CreateBucketIfNotExistsAsync();

        Tagging tagging = CreateTagging(request.OrderId.ToString());

        var objectName = CreateObjectName(request);

        var args = new PutObjectArgs()
                .WithBucket(bucketName)
                .WithTagging(tagging)
                .WithObject(objectName)
                .WithStreamData(request.Files.First().ContentStream)
                .WithObjectSize(request.Files.First().ContentStream.Length)
                .WithContentType(request.Files.First().ContentType);

        try
        {
            await minioClient.PutObjectAsync(args);
        }
        catch (Exception e)
        {
            throw new Exception("Error uploading file", e);
        }
    }

    private static string CreateObjectName(FileBucketRequest request)
    {
        return request.OrderId.ToString() + "/" + request.Files.First().Name;
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
        if (await minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName)))
            return;

        await minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucketName));
    }

}