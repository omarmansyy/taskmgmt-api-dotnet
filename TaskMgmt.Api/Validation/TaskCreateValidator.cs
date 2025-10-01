using FluentValidation;
using TaskMgmt.Api.Dtos.Tasks;

public class TaskCreateValidator : AbstractValidator<TaskCreateDto>
{
    public TaskCreateValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.ProjectId).GreaterThan(0);
    }
}
