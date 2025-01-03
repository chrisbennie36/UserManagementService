using MediatR;
using UserManagementService.Api.Domain.Results;

namespace UserManagementService.Api.Domain.Commands
{
    public record DeleteUserCommand(int userId) : IRequest<DomainResult>;
}