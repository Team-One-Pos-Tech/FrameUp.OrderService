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
    private const int ExecuteFrequencyInMinutes = 2;
    private readonly ILogger<FilePreprocessorBackgroundService> _logger;
    private readonly IServiceProvider _serviceProvider;

    public FilePreprocessorBackgroundService(
        ILogger<FilePreprocessorBackgroundService> logger,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _serviceProvider.CreateScope();
            var workbenchRepository = scope.ServiceProvider.GetRequiredService<IWorkbenchRepository>();
            var orderRepository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();
            var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();
            var fileBucketRepository = scope.ServiceProvider.GetRequiredService<IFileBucketRepository>();

            var workbenches = await workbenchRepository.ListAllWaitingWorkbenchesAsync();
            await Parallel.ForEachAsync(workbenches, stoppingToken, async (workbench, cancellationToken) =>
            {
                var order = await orderRepository.Get(workbench.OrderId);

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

                await UploadVideosFromWorkbenchAsync(
                    fileBucketRepository,
                    workbenchRepository,
                    workbench,
                    cancellationToken);

                await SubmitProcessVideoEventAsync(
                    publishEndpoint,
                    workbench.OrderId,
                    processVideoParameters,
                    cancellationToken);
            });
            
            await Task.Delay(TimeSpan.FromMinutes(ExecuteFrequencyInMinutes), stoppingToken);
        }
    }

    private async Task SubmitProcessVideoEventAsync(IPublishEndpoint publishEndpoint, Guid orderId,
        ProcessVideoParameters processVideoParameters, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Video is ready to be processed! Submitting event [ReadyToProcessVideo]");

        var readyToProcessVideo = new ReadyToProcessVideo(orderId, processVideoParameters);
        await publishEndpoint.Publish(readyToProcessVideo, cancellationToken);
    }

    private async Task UploadVideosFromWorkbenchAsync(
        IFileBucketRepository fileBucketRepository,
        IWorkbenchRepository workbenchRepository,
        Workbench workbench,
        CancellationToken cancellationToken = default)
    {
        try
        {
            workbench.Status = WorkbenchStatus.InProgress;
            await workbenchRepository.ChangeAsync(workbench);

            var requestUpload = SetupFileBucketRequest(workbench.OrderId, workbench);
            await fileBucketRepository.Upload(requestUpload);

            workbench.Status = WorkbenchStatus.Complete;
            await workbenchRepository.ChangeAsync(workbench);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Error while uploading videos!");

            workbench.Status = WorkbenchStatus.Failed;
            workbench.Error = exception.Message;
            await workbenchRepository.ChangeAsync(workbench);
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