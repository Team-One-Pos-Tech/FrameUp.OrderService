using FrameUp.OrderService.Api.Models;
using FrameUp.OrderService.Application.Models.Requests;

namespace FrameUp.OrderService.Api.Mappers;

public static class CreateProcessingOrderRequestMapper
{
    public static CreateProcessingOrderRequest Map(ProcessVideoBodyRequest request)
    {
        var processVideoRequest = new CreateProcessingOrderRequest()
        {
            CaptureInterval = request.CaptureInterval,
            ExportResolution = request.ExportResolution,
            Videos = request.Videos.Select(video => new VideoRequest
            {
                ContentStream = video.OpenReadStream(),
                Metadata = new VideoMetadataRequest
                {
                    Name = video.FileName,
                    Size = video.Length,
                    ContentType = video.ContentType
                }
            })
        };
        return processVideoRequest;
    }
}