using FrameUp.OrderService.Application.Models.Events;
using FrameUp.OrderService.Application.Models.Requests;
using FrameUp.OrderService.Domain.Entities;
using FrameUp.OrderService.Domain.Enums;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace FrameUp.OrderService.Api.Controllers;

[Route("[controller]")]
[ApiController]
public class EventsController(IPublishEndpoint publishEndpoint) : ControllerBase
{
    [HttpPut("UpdateOrderStatus/{orderId}")]
    public async Task UpdateOrderStatusAsync(
        Guid orderId, ProcessingStatus status, List<UpdatePackageItemRequest> packageItems)
    {
        var uploadedPackages = packageItems
            .Select(item => new UploadedPackageItem(item.FileName, item.Uri)).ToList();
        
        await publishEndpoint.Publish(new UpdateOrderStatusEvent
        {
            OrderId = orderId,
            Status = status,
            PackageItems = uploadedPackages
        });
    }
}
