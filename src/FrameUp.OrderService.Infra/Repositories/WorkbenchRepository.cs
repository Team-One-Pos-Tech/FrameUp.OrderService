using FrameUp.OrderService.Application.Contracts;
using FrameUp.OrderService.Domain.Entities;
using FrameUp.OrderService.Domain.Enums;
using FrameUp.OrderService.Infra.Abstractions;
using FrameUp.OrderService.Infra.Context;
using Microsoft.Extensions.Logging;

namespace FrameUp.OrderService.Infra.Repositories;

public class WorkbenchRepository : BaseRepository<Workbench, OrderServiceDbContext>, IWorkbenchRepository
{
    protected WorkbenchRepository(
        OrderServiceDbContext dbContext, 
        ILoggerFactory loggerFactory) : 
        base(dbContext, loggerFactory)
    {
    }

    public async Task<Guid> SaveAsync(Workbench workbench)
    {
        workbench.Id = Guid.NewGuid();
        
        await InsertAsync(workbench);
        await SaveChangesAsync();
        
        return workbench.Id;
    }

    public async Task ChangeAsync(Workbench workbench) 
        => await UpdateAsync(workbench);

    public async Task<IEnumerable<Workbench>> ListAllWaitingWorkbenchesAsync() 
        => await ListByPredicateAsync(workbench => workbench.Status == WorkbenchStatus.Waiting);
}