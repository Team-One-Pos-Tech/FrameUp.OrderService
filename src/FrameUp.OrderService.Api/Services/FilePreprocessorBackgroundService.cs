using FrameUp.OrderService.Application.Contracts;
using FrameUp.OrderService.Application.Models.Events;
using FrameUp.OrderService.Application.Models.Requests;
using FrameUp.OrderService.Domain.Entities;
using FrameUp.OrderService.Domain.Enums;
using MassTransit;
using Microsoft.AspNetCore.StaticFiles;

namespace FrameUp.OrderService.Api.Services;

public class FilePreprocessorBackgroundService : BackgroundService
{
    private readonly ILogger<FilePreprocessorBackgroundService> _logger;
    
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IFileBucketRepository _fileBucketRepository;
    
    private readonly IWorkbenchRepository _workbenchRepository;
    private readonly IOrderRepository _orderRepository;

    public FilePreprocessorBackgroundService(
        ILogger<FilePreprocessorBackgroundService> logger, 
        IFileBucketRepository fileBucketRepository, 
        IWorkbenchRepository workbenchRepository, 
        IOrderRepository orderRepository, 
        IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _fileBucketRepository = fileBucketRepository;
        _workbenchRepository = workbenchRepository;
        _orderRepository = orderRepository;
        _publishEndpoint = publishEndpoint;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var workbenches = await _workbenchRepository.ListAllWaitingWorkbenchesAsync();
        await Parallel.ForEachAsync(workbenches, stoppingToken, async (workbench, cancellationToken) =>
        {
            var order = await _orderRepository.Get(workbench.OrderId);

            if (order is null)
            {
                _logger.LogWarning("Order {OrderId} not found", workbench.OrderId);
                return;   
            }

            var processVideoParameters = new ProcessVideoParameters
            {
                ExportResolution = order.ExportResolution,
                CaptureInterval = order.CaptureInterval,
            };
                
            await UploadVideosFromWorkbenchAsync(workbench, cancellationToken);
            await SubmitProcessVideoEventAsync(workbench.OrderId, processVideoParameters, cancellationToken);
        });
    }
    
    private async Task SubmitProcessVideoEventAsync(Guid orderId, ProcessVideoParameters processVideoParameters, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Video is ready to be processed! Submitting event [ReadyToProcessVideo]");
        
        var readyToProcessVideo = new ReadyToProcessVideo(orderId, processVideoParameters);
        await _publishEndpoint.Publish(readyToProcessVideo, cancellationToken);
    }

    private async Task UploadVideosFromWorkbenchAsync(Workbench workbench, CancellationToken cancellationToken = default)
    {
        try
        {
            workbench.Status = WorkbenchStatus.InProgress;
            await _workbenchRepository.ChangeAsync(workbench);
            
            var requestUpload = SetupFileBucketRequest(workbench.OrderId, workbench);
            await _fileBucketRepository.Upload(requestUpload);

            workbench.Status = WorkbenchStatus.Complete;
            await _workbenchRepository.ChangeAsync(workbench);
        }
        catch (Exception exception)
        { 
            _logger.LogError(exception, "Error while uploading videos!");
            
            workbench.Status = WorkbenchStatus.Failed;
            workbench.Error = exception.Message;
            await _workbenchRepository.ChangeAsync(workbench);
        }
    }

    private FileBucketRequest SetupFileBucketRequest(Guid orderId, Workbench workbench)
    {
        var fileInfos = workbench.ListAllFileDetails();
        IList<FileRequest> fileRequests = [];
        
        foreach (var fileInfo in fileInfos)
        {
            _logger.LogInformation("Preparing to upload the file [{fileName}]", fileInfo.Name);
            using var stream = fileInfo.OpenRead();
            fileRequests.Add(new FileRequest
            {
                ContentStream = stream,
                ContentType = GetContentTypeFromFileInfo(fileInfo),
                Name = fileInfo.Name,
                Size = stream.Length
            });
        }

        return new FileBucketRequest
        {
            OrderId = orderId,
            Files = fileRequests
        };
    }

    private string GetContentTypeFromFileInfo(FileInfo fileInfo)
    {
        var provider = new FileExtensionContentTypeProvider();
        if (!provider.TryGetContentType(fileInfo.Name, out var contentType))
        {
            contentType = "application/octet-stream";
        }
        
        return contentType;
    }

}