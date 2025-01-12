using UserManagementService.Api.Domain.Commands;
using MediatR;
using Utilities.ResultPattern;
using UserManagementService.Api.Data.Entities;
using UserManagementService.Api.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace UserManagementService.Api.Domain.Handlers;

public class UpdateUserCommandHandler: IRequestHandler<UpdateUserCommand, DomainResult>
{
    private readonly IEntityRepository<User> userRepository;

    public UpdateUserCommandHandler(IEntityRepository<User> userRepository)
    {
        this.userRepository = userRepository;
    }

    public async Task<DomainResult> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        User userDataToUpdate = new User 
        {
            Id = request.userId,
            Username = request.username,
            Password = request.password,
            UpdatedUtc = DateTime.UtcNow
        };

        try
        {
            await userRepository.UpdateAsync(userDataToUpdate);
            return new DomainResult(ResponseStatus.Success);
        } 
        catch(DbUpdateException)
        {
            return new DomainResult(ResponseStatus.Error, $"Unable to update a {nameof(User)} in the database");
        }
    }
}

