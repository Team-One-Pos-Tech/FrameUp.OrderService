using FrameUp.OrderService.Application.Contracts;
using FrameUp.OrderService.Domain.Enums;
using FrameUp.OrderService.Domain.Entities;
using MassTransit;
using FrameUp.OrderService.Application.Validators;
using FrameUp.OrderService.Application.Models.Requests;
using FrameUp.OrderService.Application.Models.Responses;
using FrameUp.OrderService.Application.Models.Events;
using Microsoft.Extensions.Logging;
using MassTransit.Transports;

namespace FrameUp.OrderService.Application.UseCases;

public class CreateProcessingOrder(
    ILogger<CreateProcessingOrder> logger,
    IOrderRepository orderRepository,
    IPublishEndpoint publishEndpoint) : ICreateProcessingOrder
{
    
    public async Task<CreateProcessingOrderResponse> Execute(CreateProcessingOrderRequest request)
    {
        if (!CreateProcessingOrderValidator.IsValid(request, out var response))
            return response;

        logger.LogInformation("Starting creation of new Processing Order");

        var order = CreateOrder(request);

        order.Id = await orderRepository.Save(order);

        await UploadVideos(order.Id, request);

        logger.LogInformation("New Processing Order [{orderId}] created successfully!", order.Id);

        return new CreateProcessingOrderResponse
        {
            Id = order.Id,
            Status = order.Status
        };
    }

    private async Task UploadVideos(Guid orderId, CreateProcessingOrderRequest request)
    {
        // Should save the files streams to the local store

        await publishEndpoint.Publish(
            new UploadVideoEvent
            {
                OrderId = orderId,
                FilesNames = request.Videos.Select(video => video.Metadata.Name)
            }
        );
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