using FrameUp.OrderService.Application.Contracts;
using FrameUp.OrderService.Application.Jobs;
using FrameUp.OrderService.Application.Models.Requests;
using FrameUp.OrderService.Domain.Contracts;
using FrameUp.OrderService.Domain.Entities;
using FrameUp.OrderService.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Channels;

namespace FrameUp.OrderService.Application.Services;

public class UploadVideosService(
        ILogger<UploadVideosService> logger,
        Channel<UploadVideosJob> channel,
        IServiceProvider serviceProvider) : BackgroundService
{

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var jobs = channel.Reader.ReadAllAsync(stoppingToken);

        Order? order = null;

        await foreach (var job in jobs)
        {
            try
            {
                order = job.Order;  

                await ProcessJobAsync(job);

                //await ProcessVideos(order!);

                logger.LogInformation("UploadVideosJob has been uploaded successfully");
            }
            catch (OperationCanceledException)
            {
                await UpdateOrderStatus(order, ProcessingStatus.Failed);

                logger.LogInformation("UploadVideosService has been cancelled");
                break;
            }
            catch (Exception ex)
            {
                await UpdateOrderStatus(order, ProcessingStatus.Failed);

                logger.LogError(ex, "An error occurred while processing UploadVideosJob");
            }
        }
    }

    private async Task UpdateOrderStatus(Order? order, ProcessingStatus status)
    {
        order!.Status = status;
        using (var scope = serviceProvider.CreateScope())
        {
            var orderRepository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();
            orderRepository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();
            await orderRepository.Update(order!);
        }
    }

    private async Task ProcessJobAsync(UploadVideosJob job)
    {
        try
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var fileBucketRepository = scope.ServiceProvider.GetRequiredService<IFileBucketRepository>();

                var localStoreRepository = scope.ServiceProvider.GetRequiredService<ILocalStoreRepository>();

                Dictionary<string, Stream> files = await GetFiles(job, localStoreRepository);

                FileBucketRequest requestUpload = CreateRequestUpload(job, files);

                await fileBucketRepository.Upload(requestUpload);

            }
        }
        catch
        {
            throw;
        }
    }

    private static FileBucketRequest CreateRequestUpload(UploadVideosJob job, Dictionary<string, Stream> files)
    {
        return new FileBucketRequest
        {
            OrderId = job.Order.Id,
            Files = job.Order.Videos.Select(video => new FileRequest
            {
                ContentStream = files[video.Name],
                Name = video.Name,
                Size = video.Size,
                ContentType = video.ContentType
            })
        };
    }

    private static async Task<Dictionary<string, Stream>> GetFiles(UploadVideosJob job, ILocalStoreRepository localStoreRepository)
    {
        var files = new Dictionary<string, Stream>();

        foreach (var item in job.Order.Videos)
        {
            files[item.Name] = localStoreRepository.GetFile(job.Order.Id, item.Name);
            await localStoreRepository.DeleteFileAsync(job.Order.Id, item.Name);
        }

        return files;
    }

    //private async Task ProcessVideos(Order order)
    //{
    //    var parameters = new ProcessVideoParameters
    //    {
    //        ExportResolution = order.ExportResolution,
    //        CaptureInterval = order.CaptureInterval,
    //    };

    //    await publishEndpoint.Publish(
    //        new ReadyToProcessVideo(order.Id, parameters)
    //    );
    //}
}
