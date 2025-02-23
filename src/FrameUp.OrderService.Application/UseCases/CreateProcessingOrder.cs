using FrameUp.OrderService.Application.Contracts;
using FrameUp.OrderService.Domain.Enums;
using FrameUp.OrderService.Domain.Entities;
using MassTransit;
using FrameUp.OrderService.Application.Validators;
using FrameUp.OrderService.Application.Models.Requests;
using FrameUp.OrderService.Application.Models.Responses;
using FrameUp.OrderService.Application.Models.Events;
using Microsoft.Extensions.Logging;
using System.Threading.Channels;
using FrameUp.OrderService.Application.Jobs;
using System.Collections.Concurrent;
using FrameUp.OrderService.Application.Enums;

namespace FrameUp.OrderService.Application.UseCases;

public class CreateProcessingOrder(
    ILogger<CreateProcessingOrder> logger,
    IFileBucketRepository fileBucketRepository, 
    IOrderRepository orderRepository,
    IPublishEndpoint publishEndpoint,
    Channel<UploadVideosJob> channel,
    ConcurrentDictionary<Guid, UploadVideosStatus> statusDictionary
    ) : ICreateProcessingOrder
{
    
    public async Task<CreateProcessingOrderResponse> Execute(CreateProcessingOrderRequest request)
    {
        if (!CreateProcessingOrderValidator.IsValid(request, out var response))
            return response;

        logger.LogInformation("Starting creation of new Processing Order");

        var order = CreateOrder(request);

        order.Id = await orderRepository.Save(order);

        await UploadVideos(order, request);

        logger.LogInformation("New Processing Order [{orderId}] created successfully!", order.Id);

        return new CreateProcessingOrderResponse
        {
            Id = order.Id,
            Status = order.Status
        };
    }

    private async Task UploadVideos(Order order, CreateProcessingOrderRequest request)
    {
        var requestUpload = new FileBucketRequest
        {
            OrderId = order.Id,
            Files = request.Videos.Select(video => new FileRequest
            {
                ContentStream = video.ContentStream,
                Name = video.Metadata.Name,
                Size = video.Metadata.Size,
                ContentType = video.Metadata.ContentType
            })
        };

        var uploadJob = new UploadVideosJob(order, requestUpload);

        await channel.Writer.WriteAsync(uploadJob);
    }

    private static Order CreateOrder(CreateProcessingOrderRequest request)
    {
        return new Order()
        {
            OwnerId = request.OwnerId,
            CaptureInterval = request.CaptureInterval,
            ExportResolution = request.ExportResolution ?? ResolutionTypes.FullHD,
            Status = ProcessingStatus.Processing,
            Videos = request.Videos.Select(video => new VideoMetadata
            {
                ContentType = video.Metadata.ContentType,
                Size = video.Metadata.Size,
                Name = video.Metadata.Name
            }).ToList()
        };
    }
}