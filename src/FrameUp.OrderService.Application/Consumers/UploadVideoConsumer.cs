using FrameUp.OrderService.Application.Contracts;
using FrameUp.OrderService.Application.Models.Events;
using FrameUp.OrderService.Application.Models.Requests;
using FrameUp.OrderService.Domain.Entities;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace FrameUp.OrderService.Application.Consumers;

public class UploadVideoConsumer(
    ILogger<ProcessVideoConsumer> logger,
    IPublishEndpoint publishEndpoint,
    IOrderRepository orderRepository,
    IFileBucketRepository fileBucketRepository) : IConsumer<UploadVideoEvent>
{
    public async Task Consume(ConsumeContext<UploadVideoEvent> context)
    {
        logger.LogInformation("Uploading videos of order {orderId}", context.Message.OrderId);

        //await UploadVideos(fileBucketRepository, context);

        var order = await orderRepository.Get(context.Message.OrderId);

        await ProcessVideos(order!);

        logger.LogInformation("Finish Uploading videos of order {orderId}", context.Message.OrderId);
    }

    private static async Task UploadVideos(IFileBucketRepository fileBucketRepository, ConsumeContext<UploadVideoEvent> context)
    {
        //var requestUpload = new FileBucketRequest
        //{
        //    OrderId = context.Message.OrderId,
        //    Files = context.Message.FilesNames
        //};

        //await fileBucketRepository.Upload(requestUpload);
    }

    private async Task ProcessVideos(Order order)
    {
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
