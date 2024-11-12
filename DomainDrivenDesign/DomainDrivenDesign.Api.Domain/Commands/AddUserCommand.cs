using DomainDrivenDesign.Shared.Enums;
using MediatR;

namespace DomainDrivenDesign.Api.Domain.Commands
{
    public record AddUserCommand(string username, string password, UserRole role) : IRequest<bool>;
}