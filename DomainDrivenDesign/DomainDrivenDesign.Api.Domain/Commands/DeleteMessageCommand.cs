using MediatR;

namespace DomainDrivenDesign.Api.Domain.Commands;

public record DeleteMessageCommand(int messageId) : IRequest<bool>;
