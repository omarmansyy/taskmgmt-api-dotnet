namespace TaskMgmt.Api.Domain.Entities{

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = default!;
    public byte[] PasswordHash { get; set; } = default!;
    public byte[] PasswordSalt { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Role { get; set; } = "Employee"; // Manager / Employee
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<TaskItem> AssignedTasks { get; set; } = new List<TaskItem>();
}
}