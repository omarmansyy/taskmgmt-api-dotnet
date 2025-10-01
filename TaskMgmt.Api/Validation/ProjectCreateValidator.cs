using FluentValidation;
using TaskMgmt.Api.Dtos.Projects;

public class ProjectCreateValidator : AbstractValidator<ProjectCreateDto>
{
    public ProjectCreateValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
    }
}
