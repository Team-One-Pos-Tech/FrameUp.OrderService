using FrameUp.OrderService.Domain.Contracts;
using System.Security.Claims;

namespace FrameUp.OrderService.Api.Services;

public class AuthenticatedUser(IHttpContextAccessor httpContextAccessor) : IAuthenticatedUser
{
    private readonly ClaimsPrincipal _authenticatedUser = httpContextAccessor.HttpContext.User;

    public Guid UserId => Guid.Parse(_authenticatedUser.FindFirst("userid")?.Value ?? "");

    public string UserName => _authenticatedUser.FindFirst("username")?.Value ?? "";
}
