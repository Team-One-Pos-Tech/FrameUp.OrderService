using FrameUp.OrderService.Application.Contracts;
using FrameUp.OrderService.Domain.Entities;
using FrameUp.OrderService.Infra.Abstractions;
using FrameUp.OrderService.Infra.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FrameUp.OrderService.Infra.Repositories;

public class OrderRepository(
    OrderServiceDbContext dbContext,
    ILoggerFactory loggerFactory
    ) : BaseRepository<Order, OrderServiceDbContext>(dbContext, loggerFactory), IOrderRepository
{
    public async Task<Order?> Get(Guid orderId, Guid ownerId)
    {
        var response = await _dbSet
            .AsNoTracking()
            .Include(px => px.Videos)
            .Where(order => order.Id == orderId && order.OwnerId == ownerId)
            .FirstOrDefaultAsync();

        return response;
    }

    public async Task<IEnumerable<Order>> GetAll(Guid ownerId)
    {
        var response = await _dbSet
            .Include(px => px.Videos)
            .AsNoTracking()
            .ToListAsync();

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

    public Task Update(Order order)
    {
        throw new NotImplementedException();
    }
}