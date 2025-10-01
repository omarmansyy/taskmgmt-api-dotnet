namespace TaskMgmt.Api.Dtos.Auth;
public class RegisterRequest
{
    public string Username { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
}
