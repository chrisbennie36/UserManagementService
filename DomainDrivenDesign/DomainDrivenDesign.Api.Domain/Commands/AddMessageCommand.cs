using MediatR;

namespace DomainDrivenDesign.Api.Domain.Commands;

public record AddMessageCommand(string message) : IRequest<bool>;
