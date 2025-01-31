using FrameUp.OrderService.Application.Contracts;
using FrameUp.OrderService.Domain.Entities;
using FrameUp.OrderService.Infra.Abstractions;
using FrameUp.OrderService.Infra.Context;
using Microsoft.Extensions.Logging;

namespace FrameUp.OrderService.Infra.Repositories;

public class OrderRepository : BaseRepository<Order, OrderServiceDbContext>, IOrderRepository
{
    public OrderRepository(OrderServiceDbContext dbContext,
        ILoggerFactory loggerFactory) : base(dbContext, loggerFactory)
    {
        _expandProperties = [nameof(Order.Videos)];
    }

    public async Task<Order?> Get(Guid orderId)
    {
        return await FindByPredicateAsync(px => px.Id == orderId);
    }

    public async Task<IEnumerable<Order>> GetAll(Guid requesterId)
    {
        var response = await ListByPredicateAsync(px => px.OwnerId == requesterId);
        return response is null ? [] : response;
    }

    public async Task<Guid> Save(Order order)
    {
        order.Id = Guid.NewGuid();

        order.Videos.ToList().ForEach(video => video.OrderId = order.Id);

        await InsertAsync(order);

        await SaveChangesAsync();

        return order.Id;
    }

    public async Task Update(Order order)
    {
        _dbSet.Update(order);

        await SaveChangesAsync();
    }
}