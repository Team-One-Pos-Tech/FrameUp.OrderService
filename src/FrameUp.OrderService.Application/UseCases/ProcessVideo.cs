using FrameUp.OrderService.Application.Enums;
using FrameUp.OrderService.Application.Models;
using FrameUp.OrderService.Application.Repositories;

namespace FrameUp.OrderService.Application.UseCases;

public class ProcessVideo(IFileBucket fileBucket)
{
    private const long MaxVideoSize = 1024L * 1024L * 1024L;
    
    public async Task<ProcessVideoResponse> Execute(ProcessVideoRequest request)
    {
        if (!IsValid(request, out var response)) 
            return response;

        await fileBucket.Save(request.Video, request.VideoName);
        
        return new ProcessVideoResponse
        {
            Status = ProcessingStatus.Processing
        };
    }

    private static bool IsValid(ProcessVideoRequest request, out ProcessVideoResponse responseOut)
    {
        responseOut = new ProcessVideoResponse();

        if (request.VideoSize > MaxVideoSize)
        {
            responseOut.Status = ProcessingStatus.Refused;
            responseOut.AddNotification("Video", "Video size is too large.");
            return false;
        }
        if (request.VideoContentType != "video/mp4")
        {
            responseOut.Status = ProcessingStatus.Refused;
            responseOut.AddNotification("Video", "File type not supported.");
            return false;
        }

        return true;
    }
}