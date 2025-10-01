namespace TaskMgmt.Api.Dtos.Users;
public class UserCreateDto
{
    public string Username { get; set; } = default!;
    public string Password { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Role { get; set; } = "Employee";
}
