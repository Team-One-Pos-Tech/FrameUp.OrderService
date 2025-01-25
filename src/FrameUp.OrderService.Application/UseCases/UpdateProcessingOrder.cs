using FrameUp.OrderService.Application.Contracts;
using FrameUp.OrderService.Application.Models.Requests;
using FrameUp.OrderService.Application.Models.Responses;

namespace FrameUp.OrderService.Application.UseCases;

public class UpdateProcessingOrder(IOrderRepository orderRepository) : IUpdateProcessingOrder
{

    public async Task<UpdateProcessingOrderResponse> Execute(UpdateProcessingOrderRequest request)
    {
        var response = new UpdateProcessingOrderResponse();

        var order = await orderRepository.Get(request.OrderId);

        if (order is null)
        {
            response.AddNotification("Order", "Order not found");
            return response;
        }

        order.Status = request.Status;

        await orderRepository.Update(order);

        return response;
    }
}
