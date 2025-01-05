using UserManagementService.Api.Domain.Results;
using MediatR;
using Utilities.ResultPattern;

namespace UserManagementService.Api.Domain.Queries;

public record GetUserByIdQuery(int userId) : IRequest<DomainResult<UserResult>>;
