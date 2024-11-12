using MediatR;

namespace DomainDrivenDesign.Api.Domain.Commands
{
    public record DeleteUserCommand(int userId) : IRequest<bool>;
}