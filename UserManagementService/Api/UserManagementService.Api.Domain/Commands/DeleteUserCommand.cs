using MediatR;

namespace UserManagementService.Api.Domain.Commands
{
    public record DeleteUserCommand(int userId) : IRequest<bool>;
}