namespace TaskMgmt.Api.Dtos.Auth;
public class AuthResponse
{
    public string Token { get; set; } = default!;
    public int UserId { get; set; }
    public string Username { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string Role { get; set; } = default!;
}
