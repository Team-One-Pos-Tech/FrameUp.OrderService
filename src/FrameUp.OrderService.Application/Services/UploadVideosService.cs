using FrameUp.OrderService.Application.Contracts;
using FrameUp.OrderService.Application.Enums;
using FrameUp.OrderService.Application.Jobs;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Threading.Channels;

namespace FrameUp.OrderService.Application.Services;

public class UploadVideosService(
    ILogger<UploadVideosService> logger, 
    IFileBucketRepository fileBucketRepository,
    Channel<UploadVideosJob> channel,
    ConcurrentDictionary<Guid, UploadVideosStatus> statusDictionary
    ) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var jobs = channel.Reader.ReadAllAsync(stoppingToken);

        await foreach (var job in jobs)
        {
            try
            {
                await ProcessJobAsync(job);
            }
            catch (OperationCanceledException)
            {
                logger.LogInformation("UploadVideosService has been cancelled");
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while processing UploadVideosJob");
            }
        }
    }

    private async Task ProcessJobAsync(UploadVideosJob job)
    {
        statusDictionary[job.UploadRequest.OrderId] = UploadVideosStatus.InProgress;

        try
        {
            await fileBucketRepository.Upload(job.UploadRequest);
            statusDictionary[job.UploadRequest.OrderId] = UploadVideosStatus.Completed;
        }
        catch
        {
            statusDictionary[job.UploadRequest.OrderId] = UploadVideosStatus.Failed;
            throw;
        }
    }
}
