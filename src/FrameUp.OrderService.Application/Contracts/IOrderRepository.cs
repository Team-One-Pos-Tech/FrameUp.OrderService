using FrameUp.OrderService.Domain.Entities;

namespace FrameUp.OrderService.Application.Contracts;

public interface IOrderRepository
{
    public Task<Order> Get(Guid orderId, Guid ownerId);

    public Task<Guid> Save(Order order);
}