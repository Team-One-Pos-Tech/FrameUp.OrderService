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
    private const int MaxVideoCount = 3;
    public static readonly List<string> SupportedContentTypes = ["video/mp4"];
    
    public async Task<CreateProcessingOrderResponse> Execute(CreateProcessingOrderRequest request)
    {
        if (!IsValid(request, out var response)) 
            return response;

        var order = CreateOrder(request);
        
        order.Id = await orderRepository.Save(order);

        // How long this could take? 
        await UploadVideos(order.Id, request);

        await publishEndpoint.Publish<ReadyToProcessVideo>(new ReadyToProcessVideo(order.Id));
        
        return new CreateProcessingOrderResponse
        {
            Status = order.Status
        };
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
        
        await fileBucketRepository.Save(requestUpload);
    }

    private static bool IsValid(CreateProcessingOrderRequest request, out CreateProcessingOrderResponse responseOut)
    {
        responseOut = new CreateProcessingOrderResponse();

        if (request.Videos.Count() > MaxVideoCount)
        {
            responseOut.Status = ProcessingStatus.Refused;
            responseOut.AddNotification("Video", "Max supported videos processing is 3.");
            return false;
        }

        foreach (var video in request.Videos)
        {
            if (video.Metadata.Size > MaxVideoSize)
            {
                responseOut.Status = ProcessingStatus.Refused;
                responseOut.AddNotification("Video", "Video size is too large.");
                return false;
            }
            if (!SupportedContentTypes.Contains(video.Metadata.ContentType))
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
            Videos = request.Videos.Select(video => new VideoMetadata
            {
                ContentType = video.Metadata.ContentType,
                Size = video.Metadata.Size,
                Name = video.Metadata.Name
            })
        };
    }
}