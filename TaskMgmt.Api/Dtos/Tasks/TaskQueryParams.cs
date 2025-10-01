using TaskMgmt.Api.Domain.Enums;

namespace TaskMgmt.Api.Dtos.Tasks;
public class TaskQueryParams
{
    public TaskState? Status { get; set; }
    public TaskPriority? Priority { get; set; }
    public int? ProjectId { get; set; }
    public int? AssignedToUserId { get; set; }
    public string? Search { get; set; } // title/desc contains
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
