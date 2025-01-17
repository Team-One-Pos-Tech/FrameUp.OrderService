using FrameUp.OrderService.Application.Repositories;
using FrameUp.OrderService.Domain.Entities;

namespace FrameUp.OrderService.Infra.Repositories;

public class OrderRepository: IOrderRepository
{
    public Task<Guid> Save(Order order)
    {
        Console.WriteLine("Saving order to database");

        return Task.FromResult(Guid.NewGuid());
    }
}