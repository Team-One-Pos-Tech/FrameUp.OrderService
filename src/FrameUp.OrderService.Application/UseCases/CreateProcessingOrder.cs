using FrameUp.OrderService.Application.Contracts;
using FrameUp.OrderService.Domain.Enums;
using FrameUp.OrderService.Application.Models;
using FrameUp.OrderService.Application.Models.Consumers;
using FrameUp.OrderService.Domain.Entities;
using MassTransit;
using FrameUp.OrderService.Application.Validators;

namespace FrameUp.OrderService.Application.UseCases;

public class CreateProcessingOrder(
    IFileBucketRepository fileBucketRepository, 
    IOrderRepository orderRepository,
    IPublishEndpoint publishEndpoint) : ICreateProcessingOrder
{
    
    public async Task<CreateProcessingOrderResponse> Execute(CreateProcessingOrderRequest request)
    {
        if (!CreateProcessingOrderValidator.IsValid(request, out var response))
            return response;

        var order = CreateOrder(request);

        order.Id = await orderRepository.Save(order);

        // How long this could take? 
        await UploadVideos(order.Id, request);

        await ProcessVideos(order, request);

        return new CreateProcessingOrderResponse
        {
            Id = order.Id,
            Status = order.Status
        };
    }

    private async Task ProcessVideos(Order order, CreateProcessingOrderRequest request)
    {
        var parameters = new ProcessVideoParameters
        {
            ExportResolution = request.ExportResolution,
            CaptureInterval = request.CaptureInterval,
        };

        await publishEndpoint.Publish(
            new ReadyToProcessVideo(order.Id, parameters)
        );
    }

    private async Task UploadVideos(Guid orderId, CreateProcessingOrderRequest request)
    {
        var requestUpload = new FileBucketRequest
        {
            OrderId = orderId,
            Files = request.Videos.Select(video => new FileRequest
            {
                ContentStream = video.ContentStream,
                Name = video.Metadata.Name,
                Size = video.Metadata.Size,
                ContentType = video.Metadata.ContentType
            })
        };
        
        await fileBucketRepository.Upload(requestUpload);
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