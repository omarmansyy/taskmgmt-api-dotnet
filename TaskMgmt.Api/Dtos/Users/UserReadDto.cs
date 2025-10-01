namespace TaskMgmt.Api.Dtos.Users;
public class UserReadDto
{
    public int Id { get; set; }
    public string Username { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Role { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
}
