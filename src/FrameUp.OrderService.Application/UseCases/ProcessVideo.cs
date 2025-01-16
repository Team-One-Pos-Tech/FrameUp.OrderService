using FrameUp.OrderService.Application.Enums;
using FrameUp.OrderService.Application.Models;
using FrameUp.OrderService.Application.Repositories;

namespace FrameUp.OrderService.Application.UseCases;

public class ProcessVideo(IFileBucket fileBucket)
{
    const long MaxVideoSize = 1024L * 1024L * 1024L;
    
    public async Task<ProcessVideoResponse> Execute(ProcessVideoRequest request)
    {
        var response = new ProcessVideoResponse();
        
        if (request.VideoSize > MaxVideoSize)
        {
            response.Status = ProcessingStatus.Refused;
            response.AddNotification("Video", "Video size is too large");
            return response;
        }
        
        await fileBucket.Save(request.Video, request.VideoName);
        
        return new ProcessVideoResponse
        {
            Status = ProcessingStatus.Processing
        };
    }
}