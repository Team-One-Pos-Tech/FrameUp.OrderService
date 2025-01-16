using FrameUp.OrderService.Domain.Enums;
using FrameUp.OrderService.Application.Models;
using FrameUp.OrderService.Application.Repositories;
using FrameUp.OrderService.Domain.Entities;

namespace FrameUp.OrderService.Application.UseCases;

public class ProcessVideo(IFileBucketRepository fileBucketRepository, IOrderRepository orderRepository)
{
    private const long MaxVideoSize = 1024L * 1024L * 1024L;
    
    public async Task<ProcessVideoResponse> Execute(ProcessVideoRequest request)
    {
        if (!IsValid(request, out var response)) 
            return response;

        var order = CreateOrder(request);
        
        await orderRepository.Save(order);

        await fileBucketRepository.Save(request.Video, request.VideoMetadata);
        
        return new ProcessVideoResponse
        {
            Status = order.Status
        };
    }

    private static bool IsValid(ProcessVideoRequest request, out ProcessVideoResponse responseOut)
    {
        responseOut = new ProcessVideoResponse();

        if (request.VideoMetadata.Size > MaxVideoSize)
        {
            responseOut.Status = ProcessingStatus.Refused;
            responseOut.AddNotification("Video", "Video size is too large.");
            return false;
        }
        if (request.VideoMetadata.ContentType != "video/mp4")
        {
            responseOut.Status = ProcessingStatus.Refused;
            responseOut.AddNotification("Video", "File type not supported.");
            return false;
        }

        return true;
    }
    
    private static Order CreateOrder(ProcessVideoRequest request)
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