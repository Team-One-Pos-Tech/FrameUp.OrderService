using FrameUp.OrderService.Application.Models.Requests;
using FrameUp.OrderService.Application.Models.Responses;

namespace FrameUp.OrderService.Application.Contracts;

public interface IUpdateProcessingOrder
{
    Task<UpdateProcessingOrderResponse> Execute(UpdateProcessingOrderRequest request);
}