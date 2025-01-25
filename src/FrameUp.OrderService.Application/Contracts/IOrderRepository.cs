using FrameUp.OrderService.Application.Models;
using FrameUp.OrderService.Domain.Entities;

namespace FrameUp.OrderService.Application.Contracts;

public interface IOrderRepository
{
    public Task<Order?> Get(Guid orderId);
    Task<IEnumerable<Order>> GetAll(Guid ownerId);
    public Task<Guid> Save(Order order);
    public Task Update(Order order);
}