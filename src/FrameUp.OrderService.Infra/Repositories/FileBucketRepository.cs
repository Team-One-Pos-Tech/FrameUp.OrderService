using FrameUp.OrderService.Application.Contracts;
using FrameUp.OrderService.Application.Models;
using Minio;
using Minio.DataModel.Args;

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

        var args = new PutObjectArgs()
                .WithBucket(bucketName)
                .WithObject(request.OrderId.ToString())
                .WithStreamData(request.Files[0].ContentStream)
                .WithObjectSize(request.Files[0].ContentStream.Length)
                .WithContentType(request.Files[0].ContentType);

        try
        {
            await minioClient.PutObjectAsync(args);
        }
        catch (Exception e)
        {
            throw new Exception("Error uploading file", e);
        }
    }

    private async Task CreateBucketIfNotExistsAsync()
    {
        if (await minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName)))
            return;

        await minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucketName));
    }

}