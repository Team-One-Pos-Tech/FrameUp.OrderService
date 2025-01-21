using FrameUp.OrderService.Application.Models;

namespace FrameUp.OrderService.Application.Contracts
{
    public interface IGetProcessingOrder
    {
        Task<GetProcessingOrderResponse?> GetById(GetProcessingOrderRequest request);
    }
}