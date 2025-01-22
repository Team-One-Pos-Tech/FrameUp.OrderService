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

    public async Task<Guid> Save(Order order)
    {
        order.Id = Guid.NewGuid();

        await InsertAsync(order);

        return order.Id;
    }
}