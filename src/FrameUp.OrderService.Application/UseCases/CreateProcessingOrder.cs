using FrameUp.OrderService.Domain.Enums;
using FrameUp.OrderService.Application.Models;
using FrameUp.OrderService.Application.Models.Consumers;
using FrameUp.OrderService.Application.Repositories;
using FrameUp.OrderService.Domain.Entities;
using MassTransit;

namespace FrameUp.OrderService.Application.UseCases;

public class CreateProcessingOrder(
    IFileBucketRepository fileBucketRepository, 
    IOrderRepository orderRepository,
    IPublishEndpoint publishEndpoint)
{
    private const long MaxVideoSize = 1024L * 1024L * 1024L;
    
    public async Task<CreateProcessingOrderResponse> Execute(CreateProcessingOrderRequest request)
    {
        if (!IsValid(request, out var response)) 
            return response;

        var order = CreateOrder(request);
        
        order.Id = await orderRepository.Save(order);

        await fileBucketRepository.Save(request.Video, request.VideoMetadata);

        await publishEndpoint.Publish<ReadyToProcessVideo>(new ReadyToProcessVideo(order.Id));
        
        return new CreateProcessingOrderResponse
        {
            Status = order.Status
        };
    }

    private static bool IsValid(CreateProcessingOrderRequest request, out CreateProcessingOrderResponse responseOut)
    {
        responseOut = new CreateProcessingOrderResponse();

        foreach (var video in request.Videos)
        {
            if (video.Metadata.Size > MaxVideoSize)
            {
                responseOut.Status = ProcessingStatus.Refused;
                responseOut.AddNotification("Video", "Video size is too large.");
                return false;
            }
            if (video.Metadata.ContentType != "video/mp4")
            {
                responseOut.Status = ProcessingStatus.Refused;
                responseOut.AddNotification("Video", "File type not supported.");
                return false;
            }
        }

        return true;
    }
    
    private static Order CreateOrder(CreateProcessingOrderRequest request)
    {
        return new Order()
        {
            Status = ProcessingStatus.Processing,
            VideoMetadata = new VideoMetadata
            {
                ContentType = request.VideoMetadata.ContentType,
                Size = request.VideoMetadata.Size,
                Name = request.VideoMetadata.Name
            }
        };
    }
}