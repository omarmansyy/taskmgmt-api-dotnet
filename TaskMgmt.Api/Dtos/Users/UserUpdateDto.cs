namespace TaskMgmt.Api.Dtos.Users;
public class UserUpdateDto
{
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Role { get; set; } = "Employee"; // only Manager can change role
}
