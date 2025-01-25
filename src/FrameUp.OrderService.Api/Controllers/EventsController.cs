using FrameUp.OrderService.Application.Models.Events;
using FrameUp.OrderService.Domain.Enums;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace FrameUp.OrderService.Api.Controllers;

[Route("events/[controller]")]
[ApiController]
public class EventsController(IPublishEndpoint publishEndpoint) : ControllerBase
{
    [HttpPut("{orderId}")]
    public async Task UpdateOrderStatusAsync(Guid orderId, ProcessingStatus status)
    {
        await publishEndpoint.Publish(new UpdateOrderStatusEvent(orderId, status));
    }
}
