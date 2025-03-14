using FrameUp.OrderService.Application.Contracts;
using FrameUp.OrderService.Domain.Enums;
using FrameUp.OrderService.Domain.Entities;
using FrameUp.OrderService.Application.Validators;
using FrameUp.OrderService.Application.Models.Requests;
using FrameUp.OrderService.Application.Models.Responses;
using Microsoft.Extensions.Logging;
using FrameUp.OrderService.Application.Jobs;
using FrameUp.OrderService.Domain.Contracts;

namespace FrameUp.OrderService.Application.UseCases;

public class CreateProcessingOrder(
    ILogger<CreateProcessingOrder> logger,
    IOrderRepository orderRepository,
    IUploadVideosChannel uploadVideosChannel,
    ILocalStoreRepository localStoreRepository
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
        foreach (var item in request.Videos)
        {
            await localStoreRepository.SaveFileAsync(order.Id, item.Metadata.Name, item.ContentStream);
        }

        var uploadJob = new UploadVideosJob(order);

        await uploadVideosChannel.WriteAsync(uploadJob);
    }

    private static Order CreateOrder(CreateProcessingOrderRequest request)
    {
        return new Order()
        {
            OwnerId = request.OwnerId,
            CaptureInterval = request.CaptureInterval,
            ExportResolution = request.ExportResolution ?? ResolutionTypes.FullHD,
            Status = ProcessingStatus.Received,
            Videos = request.Videos.Select(video => new VideoMetadata
            {
                ContentType = video.Metadata.ContentType,
                Size = video.Metadata.Size,
                Name = video.Metadata.Name
            }).ToList()
        };
    }
}