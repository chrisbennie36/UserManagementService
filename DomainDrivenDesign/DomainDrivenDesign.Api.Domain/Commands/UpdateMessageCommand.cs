using MediatR;

namespace DomainDrivenDesign.Api.Domain.Commands;

public record UpdateMessageCommand(int messageId, string messageText) : IRequest<bool>;
