using FrameUp.OrderService.Application.Models.Requests;
using FrameUp.OrderService.Application.Models.Responses;

namespace FrameUp.OrderService.Application.Contracts;

public interface ICreateProcessingOrder
{
    public Task<CreateProcessingOrderResponse> Execute(CreateProcessingOrderRequest request);
}