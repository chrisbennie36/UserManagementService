using UserManagementService.Api.Domain.Results;
using MediatR;

namespace UserManagementService.Api.Domain.Queries;

public record GetUserByIdQuery(int userId) : IRequest<UserResult?>;
