using UserManagementService.Shared.Enums;
using MediatR;

namespace UserManagementService.Api.Domain.Commands
{
    public record AddUserCommand(string username, string password, UserRole role) : IRequest<bool>;
}