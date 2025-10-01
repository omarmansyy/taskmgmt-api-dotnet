using TaskMgmt.Api.Domain.Entities;

namespace TaskMgmt.Api.Services.Interfaces;
public interface IJwtTokenService
{
    string CreateToken(User user);
}
