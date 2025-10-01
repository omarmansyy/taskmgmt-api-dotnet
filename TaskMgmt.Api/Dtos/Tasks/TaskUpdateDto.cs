using TaskMgmt.Api.Domain.Enums;

namespace TaskMgmt.Api.Dtos.Tasks;
public class TaskUpdateDto
{
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public TaskPriority Priority { get; set; }
    public TaskState Status { get; set; }
    public int? AssignedToUserId { get; set; }
    public DateTime? DueDate { get; set; }
}
