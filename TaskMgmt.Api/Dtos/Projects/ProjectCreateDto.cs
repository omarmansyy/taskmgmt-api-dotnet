namespace TaskMgmt.Api.Dtos.Projects;
public class ProjectCreateDto
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
}
