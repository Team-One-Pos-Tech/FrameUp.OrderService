using FrameUp.OrderService.Application.Contracts;
using FrameUp.OrderService.Domain.Entities;
using FrameUp.OrderService.Infra.Abstractions;
using FrameUp.OrderService.Infra.Context;
using Microsoft.Extensions.Logging;

namespace FrameUp.OrderService.Infra.Repositories;

public class OrderRepository(
    OrderServiceDbContext dbContext,
    ILoggerFactory loggerFactory
    ) : BaseRepository<Order, OrderServiceDbContext>(dbContext, loggerFactory), IOrderRepository
{
    public async Task<Order?> Get(Guid orderId, Guid ownerId)
    {
        var response = await FindByPredicateAsync(order => order.Id == orderId && order.OwnerId == ownerId);

        return response is null ? null : response;
    }

    public Task<IEnumerable<Order>> GetAll(Guid ownerId)
    {
        throw new NotImplementedException();
    }

    public Task<Guid> Save(Order order)
    {
        Console.WriteLine("Saving order to database");

        return Task.FromResult(Guid.NewGuid());
    }
}