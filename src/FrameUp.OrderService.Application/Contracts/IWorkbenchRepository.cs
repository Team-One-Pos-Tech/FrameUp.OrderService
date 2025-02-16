using FrameUp.OrderService.Domain.Entities;

namespace FrameUp.OrderService.Application.Contracts;

public interface IWorkbenchRepository
{
    Task<Guid> SaveAsync(Workbench workbench);
    Task ChangeAsync(Workbench workbench);

    Task<IEnumerable<Workbench>> ListAllWaitingWorkbenchesAsync();
}