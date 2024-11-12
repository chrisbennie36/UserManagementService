using DomainDrivenDesign.Api.Domain.Results;
using MediatR;

namespace DomainDrivenDesign.Api.Domain.Queries;

public record GetUserByUsernameAndPasswordQuery(string username, string password) : IRequest<UserResult?>;
