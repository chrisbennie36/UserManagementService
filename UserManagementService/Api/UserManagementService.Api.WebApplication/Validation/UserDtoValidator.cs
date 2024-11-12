using UserManagementService.Api.WebApplication.Dtos;
using FluentValidation;

namespace UserManagementService.Api.WebApplication.Validation;

public class UserDtoValidator : AbstractValidator<UserDto>
{
    const int UsernameMinLength = 0;
    const int UsernameMaxLength = 200;

    public UserDtoValidator()
    {
        RuleFor(x => x.Username).Length(UsernameMinLength, UsernameMaxLength);
    }
}
