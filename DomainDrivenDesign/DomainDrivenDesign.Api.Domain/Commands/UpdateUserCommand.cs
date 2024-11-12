using DomainDrivenDesign.Shared.Enums;
using MediatR;

namespace DomainDrivenDesign.Api.Domain.Commands;

public record UpdateUserCommand(int userId, string username, string password, UserRole role) : IRequest<bool>;
