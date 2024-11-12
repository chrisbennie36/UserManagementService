using DomainDrivenDesign.Api.WebApplication.Dtos;
using FluentValidation;

namespace DomainDrivenDesign.Api.WebApplication.Validation;

public class MessageDtoValidator : AbstractValidator<MessageDto>
{
    public MessageDtoValidator()
    {
        RuleFor(x => x.MessageText).Length(0, 10000);
    }
}
