using FrameUp.OrderService.Application.Models.Requests;
using FrameUp.OrderService.Application.Models.Responses;
using FrameUp.OrderService.Domain.Enums;

namespace FrameUp.OrderService.Application.Validators;

public static class CreateProcessingOrderValidator
{
    private const long MaxVideoSize = 1024L * 1024L * 1024L;
    private const int MaxVideoCount = 3;
    private static readonly List<string> SupportedContentTypes = ["video/mp4"];

    public static bool IsValid(CreateProcessingOrderRequest request, out CreateProcessingOrderResponse responseOut)
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
}