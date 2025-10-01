using FluentValidation;
using TaskMgmt.Api.Dtos.Users;

public class UserCreateValidator : AbstractValidator<UserCreateDto>
{
    public UserCreateValidator()
    {
        RuleFor(x => x.Username).NotEmpty().MinimumLength(3);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Role).NotEmpty().Must(r => r is "Manager" or "Employee");
    }
}
