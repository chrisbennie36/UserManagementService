using UserManagementService.Shared.Enums;
using MediatR;
using Utilities.ResultPattern;

namespace UserManagementService.Api.Domain.Commands;

public record UpdateUserCommand(int userId, string username, string password, UserRole role) : IRequest<DomainResult>;
