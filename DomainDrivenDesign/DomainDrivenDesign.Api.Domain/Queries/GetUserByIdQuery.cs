using DomainDrivenDesign.Api.Domain.Results;
using MediatR;

namespace DomainDrivenDesign.Api.Domain.Queries;

public record GetUserByIdQuery(int userId) : IRequest<UserResult?>;
