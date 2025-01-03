using UserManagementService.Shared.Enums;
using MediatR;
using UserManagementService.Api.Domain.Results;

namespace UserManagementService.Api.Domain.Commands;

public record UpdateUserCommand(int userId, string username, string password, UserRole role) : IRequest<DomainResult>;
