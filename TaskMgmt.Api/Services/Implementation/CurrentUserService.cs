using System.Security.Claims;
using TaskMgmt.Api.Services.Interfaces;

namespace TaskMgmt.Api.Services.Implementations;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _http;
    public CurrentUserService(IHttpContextAccessor http) => _http = http;

    public int? UserId =>
        int.TryParse(_http.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;

    public string? Role => _http.HttpContext?.User.FindFirstValue(ClaimTypes.Role);
}
