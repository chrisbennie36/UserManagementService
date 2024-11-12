using UserManagementService.Api.Domain.Results;
using MediatR;

namespace UserManagementService.Api.Domain.Queries;

public record GetUserByUsernameAndPasswordQuery(string username, string password) : IRequest<UserResult?>;
