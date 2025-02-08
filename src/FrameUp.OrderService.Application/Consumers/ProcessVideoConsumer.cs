using FrameUp.OrderService.Application.Contracts;
using FrameUp.OrderService.Application.Models.Events;
using FrameUp.OrderService.Application.Models.Requests;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace FrameUp.OrderService.Application.Consumers;

public class ProcessVideoConsumer(
    ILogger<ProcessVideoConsumer> logger,
    IUpdateProcessingOrder updateProcessingOrder
    ) : IConsumer<UpdateOrderStatusEvent>
{
    public async Task Consume(ConsumeContext<UpdateOrderStatusEvent> context)
    {
        var response = await updateProcessingOrder.Execute(new UpdateProcessingOrderRequest
        {
            OrderId = context.Message.OrderId,
            Status = context.Message.Status,
            Packages = context.Message.PackageItems
                .Select(x => new UpdatePackageItemRequest(x.FileName,x.Uri)).ToArray()
        });

        if (response.IsValid)
        {
            logger.LogInformation("Status order processing has been updated to [{status}]", context.Message.Status);
        }
        else 
        {
            logger.LogError("Error updating order status: {errors}", response.Notifications);
        }
    }
}
