using FrameUp.OrderService.Application.Models;

namespace FrameUp.OrderService.Application.Contracts;

public interface ICreateProcessingOrder
{
    public Task<CreateProcessingOrderResponse> Execute(CreateProcessingOrderRequest request);
}