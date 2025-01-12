using UserManagementService.Api.Domain.Commands;
using MediatR;
using Utilities.ResultPattern;
using UserManagementService.Api.Data.Entities;
using UserManagementService.Api.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace UserManagementService.Api.Domain.Handlers;

public class DeleteUserCommandHandler: IRequestHandler<DeleteUserCommand, DomainResult>
{
    private readonly IEntityRepository<User> userRepository;

    public DeleteUserCommandHandler(IEntityRepository<User> userRepository)
    {
        this.userRepository = userRepository;
    }

    public async Task<DomainResult> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await userRepository.DeleteAsync(request.userId);
        }
        catch(DbUpdateException)
        {
            return new DomainResult(ResponseStatus.Error, $"Unable to delete a {nameof(User)} from the database");
        }

        return new DomainResult(ResponseStatus.Success);
    }
}

