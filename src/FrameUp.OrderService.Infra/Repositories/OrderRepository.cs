using FrameUp.OrderService.Application.Contracts;
using FrameUp.OrderService.Domain.Entities;

namespace FrameUp.OrderService.Infra.Repositories;

public class OrderRepository: IOrderRepository
{
    public Task<Order> Get(Guid orderId, Guid ownerId)
    {
        throw new NotImplementedException();
    }

    public Task<Guid> Save(Order order)
    {
        Console.WriteLine("Saving order to database");

        return Task.FromResult(Guid.NewGuid());
    }
}