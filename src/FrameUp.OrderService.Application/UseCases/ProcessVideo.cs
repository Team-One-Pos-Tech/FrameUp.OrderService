using FrameUp.OrderService.Application.Enums;
using FrameUp.OrderService.Application.Models;
using FrameUp.OrderService.Application.Repositories;

namespace FrameUp.OrderService.Application.UseCases;

public class ProcessVideo(IFileBucket fileBucket)
{
    public async Task<ProcessVideoResponse> Execute(ProcessVideoRequest request)
    {
        await fileBucket.Save(request.Video, request.VideoName);
        
        return new ProcessVideoResponse
        {
            Status = ProcessingStatus.Received
        };
    }
}