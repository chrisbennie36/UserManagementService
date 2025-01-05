using MediatR;
using Utilities.ResultPattern;

namespace UserManagementService.Api.Domain.Commands
{
    public record DeleteUserCommand(int userId) : IRequest<DomainResult>;
}