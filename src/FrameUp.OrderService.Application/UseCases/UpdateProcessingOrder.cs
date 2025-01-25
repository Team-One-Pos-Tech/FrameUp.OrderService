using FrameUp.OrderService.Application.Contracts;
using FrameUp.OrderService.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrameUp.OrderService.Application.UseCases;

public class UpdateProcessingOrderResponse { }

public class UpdateProcessingOrder(IOrderRepository orderRepository)
{

    public async Task<UpdateProcessingOrderResponse> Execute(UpdateProcessingOrderRequest request)
    {
        var order = await orderRepository.Get(request.OrderId, request.OwnerId);

        order.Status = request.Status;

        await orderRepository.Update(order);

        return new UpdateProcessingOrderResponse();
    }
}
