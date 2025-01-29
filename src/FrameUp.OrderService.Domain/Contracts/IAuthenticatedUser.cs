namespace FrameUp.OrderService.Domain.Contracts;

public interface IAuthenticatedUser
{
    Guid UserId { get; }

    string UserName { get; }
}
