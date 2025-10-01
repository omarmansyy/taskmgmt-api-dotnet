namespace TaskMgmt.Api.Services.Interfaces;
public interface ICurrentUserService
{
    int? UserId { get; }
    string? Role { get; }
}
