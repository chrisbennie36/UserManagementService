using UserManagementService.Api.Domain.Results;
using MediatR;
using Utilities.ResultPattern;

namespace UserManagementService.Api.Domain.Queries;

public record GetUserByUserCredentialsQuery(string username, string password) : IRequest<DomainResult<UserResult>>;
