using DomainDrivenDesign.Api.Domain.Results;
using MediatR;

namespace DomainDrivenDesign.Api.Domain.Queries;

public record GetMessageByIdQuery(int messageId) : IRequest<MessageResult?>;
