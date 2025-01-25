using FrameUp.OrderService.Application.Models;

namespace FrameUp.OrderService.Application.Contracts
{
    public interface IGetProcessingOrder
    {
        Task<IEnumerable<GetProcessingOrderResponse>> GetAll(GetProcessingOrderRequest getProcessingOrderRequest);

        Task<GetProcessingOrderResponse?> GetById(GetProcessingOrderRequest request);
    }
}