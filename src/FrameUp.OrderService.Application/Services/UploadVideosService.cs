using FrameUp.OrderService.Application.Contracts;
using FrameUp.OrderService.Application.Enums;
using FrameUp.OrderService.Application.Jobs;
using FrameUp.OrderService.Domain.Entities;
using FrameUp.OrderService.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Threading.Channels;

namespace FrameUp.OrderService.Application.Services;

public class UploadVideosService : BackgroundService
{
    private readonly ILogger<UploadVideosService> logger;
    private readonly Channel<UploadVideosJob> channel;
    private readonly ConcurrentDictionary<Guid, UploadVideosStatus> statusDictionary;
    private readonly IServiceProvider serviceProvider;
    private IFileBucketRepository fileBucketRepository;
    private IOrderRepository orderRepository;

    public UploadVideosService(
        ILogger<UploadVideosService> logger,
        Channel<UploadVideosJob> channel,
        ConcurrentDictionary<Guid, UploadVideosStatus> statusDictionary,
        IServiceProvider serviceProvider)
    {
        this.logger = logger;
        this.channel = channel;
        this.statusDictionary = statusDictionary;
        this.serviceProvider = serviceProvider;
    }

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
            orderRepository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();
            await orderRepository.Update(order!);
        }
    }

    private async Task ProcessJobAsync(UploadVideosJob job)
    {
        statusDictionary[job.UploadRequest.OrderId] = UploadVideosStatus.InProgress;

        try
        {
            using (var scope = serviceProvider.CreateScope())
            {
                fileBucketRepository = scope.ServiceProvider.GetRequiredService<IFileBucketRepository>();

                await fileBucketRepository.Upload(job.UploadRequest);
            }

            statusDictionary[job.UploadRequest.OrderId] = UploadVideosStatus.Completed;
        }
        catch
        {
            statusDictionary[job.UploadRequest.OrderId] = UploadVideosStatus.Failed;
            throw;
        }
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
