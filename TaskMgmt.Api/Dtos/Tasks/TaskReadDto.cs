using TaskMgmt.Api.Domain.Enums;

namespace TaskMgmt.Api.Dtos.Tasks;
public class TaskReadDto
{
    public int Id { get; set; }
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public TaskState Status { get; set; }
    public TaskPriority Priority { get; set; }
    public int ProjectId { get; set; }
    public int CreatedByUserId { get; set; }
    public int? AssignedToUserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? DueDate { get; set; }
}
