using TaskMgmt.Api.Domain.Enums;

namespace TaskMgmt.Api.Dtos.Tasks;
public class TaskCreateDto
{
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
    public int ProjectId { get; set; }
    public int? AssignedToUserId { get; set; }
    public DateTime? DueDate { get; set; }
}
