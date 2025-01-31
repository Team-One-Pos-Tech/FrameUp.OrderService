using FrameUp.OrderService.Application.Models.Requests;
using FrameUp.OrderService.Application.Models.Responses;

namespace FrameUp.OrderService.Application.Contracts
{
    public interface IGetProcessingOrder
    {
        Task<IEnumerable<GetProcessingOrderResponse>> GetAll(GetProcessingOrderRequest getProcessingOrderRequest);

        Task<GetProcessingOrderResponse> GetById(GetProcessingOrderRequest request);
    }
}