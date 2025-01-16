using FrameUp.OrderService.Domain.Entities;

namespace FrameUp.OrderService.Application.Repositories;

public interface IOrderRepository
{
    public Task Save(Order order);
}