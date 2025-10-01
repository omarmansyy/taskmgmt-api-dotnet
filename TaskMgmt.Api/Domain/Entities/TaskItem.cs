using TaskMgmt.Api.Domain.Enums;

namespace TaskMgmt.Api.Domain.Entities;

public class TaskItem
{
    public string? ImageUrl { get; set; } // optional exam image
    public int Id { get; set; }
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public TaskState Status { get; set; } = TaskState.ToDo;
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;

    public int ProjectId { get; set; }
    public Project Project { get; set; } = default!;

    public int CreatedByUserId { get; set; }
    public User CreatedByUser { get; set; } = default!;

    public int? AssignedToUserId { get; set; }
    public User? AssignedToUser { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DueDate { get; set; } // for bonus job
}
