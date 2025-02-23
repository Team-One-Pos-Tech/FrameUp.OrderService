using FrameUp.OrderService.Application.Contracts;
using FrameUp.OrderService.Application.Jobs;
using FrameUp.OrderService.Application.Models.Events;
using FrameUp.OrderService.Application.Models.Requests;
using FrameUp.OrderService.Domain.Contracts;
using FrameUp.OrderService.Domain.Entities;
using FrameUp.OrderService.Domain.Enums;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Channels;

namespace FrameUp.OrderService.Application.Services;

public class UploadVideosService(
        ILogger<UploadVideosService> logger,
        IUploadVideosChannel channel,
        IServiceProvider serviceProvider) : BackgroundService
{
    public async Task StartExecuteAsync(CancellationToken stoppingToken)
    {
        await ExecuteAsync(stoppingToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var jobs = channel.ReadAllAsync(stoppingToken);

        Order? order = null;

        await foreach (var job in jobs)
        {
            try
            {
                logger.LogInformation("Uploading videos started");

                await UpdateOrderStatus(job.Order, ProcessingStatus.Uploading);

                await UploadVideosAsync(job);

                logger.LogInformation("Videos has been uploaded successfully");

                await ProcessVideos(job.Order!);

                await UpdateOrderStatus(job.Order, ProcessingStatus.Processing);
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
        using (var scope = serviceProvider.CreateScope())
        {
            var orderRepository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();
            
            order!.Status = status;

            await orderRepository.Update(order!);
        }
    }

    private async Task UploadVideosAsync(UploadVideosJob job)
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

    private async Task ProcessVideos(Order order)
    {

        using (var scope = serviceProvider.CreateScope())
        {
            var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

            var parameters = new ProcessVideoParameters
            {
                ExportResolution = order.ExportResolution,
                CaptureInterval = order.CaptureInterval,
            };
       
            await publishEndpoint.Publish(
                new ReadyToProcessVideo(order.Id, parameters)
            );
        }
    }
}
