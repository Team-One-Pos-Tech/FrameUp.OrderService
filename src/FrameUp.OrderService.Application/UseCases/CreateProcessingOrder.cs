using FrameUp.OrderService.Application.Contracts;
using FrameUp.OrderService.Domain.Enums;
using FrameUp.OrderService.Domain.Entities;
using FrameUp.OrderService.Application.Validators;
using FrameUp.OrderService.Application.Models.Requests;
using FrameUp.OrderService.Application.Models.Responses;
using Microsoft.Extensions.Logging;

namespace FrameUp.OrderService.Application.UseCases;

public class CreateProcessingOrder : ICreateProcessingOrder
{
    private readonly ILogger<CreateProcessingOrder> _logger;
    private readonly IOrderRepository _orderRepository;
    private readonly IWorkbenchRepository _workbenchRepository;

    public CreateProcessingOrder(
        ILogger<CreateProcessingOrder> logger, 
        IOrderRepository orderRepository, 
        IWorkbenchRepository workbenchRepository)
    {
        _logger = logger;
        _orderRepository = orderRepository;
        _workbenchRepository = workbenchRepository;
    }

    public async Task<CreateProcessingOrderResponse> Execute(CreateProcessingOrderRequest request)
    {
        if (!CreateProcessingOrderValidator.IsValid(request, out var response))
            return response;

        _logger.LogInformation("Starting creation of new Processing Order");

        var order = CreateOrder(request);
        order.Id = await _orderRepository.Save(order);

        await SetupFiles(order.Id, request.Videos);
        
        _logger.LogInformation("New Processing Order [{orderId}] created successfully!", order.Id);
        return new CreateProcessingOrderResponse
        {
            Id = order.Id,
            Status = order.Status
        };
    }

    private async Task SetupFiles(Guid orderId, IEnumerable<VideoRequest> videoRequests)
    {
        _logger.LogInformation("Storing video files for order [{orderId}] to be upload in background!", orderId);
        
        var workBench = Workbench.InitializeFor(orderId);
        foreach (var videoRequest in videoRequests)
        {
            _logger.LogInformation("Storing file [{videoName}] for order [{orderId}] at [{location}]", videoRequest.Metadata.Name, orderId, workBench.Location);
            await workBench.StoreStreamAsFileAsync(videoRequest.Metadata.Name, videoRequest.ContentStream);
        }

        await _workbenchRepository.SaveAsync(workBench);
    }

    private static Order CreateOrder(CreateProcessingOrderRequest request)
    {
        return new Order
        {
            OwnerId = request.OwnerId,
            CaptureInterval = request.CaptureInterval,
            ExportResolution = request.ExportResolution ?? ResolutionTypes.FullHD,
            Status = ProcessingStatus.Processing,
            Videos = request.Videos.Select(video => new VideoMetadata
            {
                ContentType = video.Metadata.ContentType,
                Size = video.Metadata.Size,
                Name = video.Metadata.Name
            }).ToList()
        };
    }
}