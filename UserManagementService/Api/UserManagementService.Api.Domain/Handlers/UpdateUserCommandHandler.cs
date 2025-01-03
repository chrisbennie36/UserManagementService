using UserManagementService.Api.Data;
using UserManagementService.Api.Domain.Commands;
using MediatR;
using Serilog;
using UserManagementService.Api.Domain.Results;

namespace UserManagementService.Api.Domain.Handlers;

public class UpdateUserCommandHandler: IRequestHandler<UpdateUserCommand, DomainResult>
{
    private readonly AppDbContext appDbContext;

    public UpdateUserCommandHandler(AppDbContext appDbContext)
    {
        this.appDbContext = appDbContext;
    }

    public async Task<DomainResult> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            User? existingUser = await appDbContext.FindAsync<User>(request.userId);

            if(existingUser == null)
            {
                Log.Error($"Coudln't find a {nameof(User)} to update for ID {request.userId}");
                return new DomainResult(ResponseStatus.NotFound, $"Couldn't find a {nameof(User)} to update");
            }

            existingUser.Username = request.username;
            existingUser.Password = request.password;
            existingUser.Role = request.role;
            existingUser.UpdatedUtc = DateTime.UtcNow;

            await appDbContext.SaveChangesAsync();
            return new DomainResult(ResponseStatus.Success);
        }
        catch(Exception e)
        {
            string errorMessage = $"Error when writing a {nameof(User)} to the database: {e.Message}";
            Log.Error(errorMessage);
            return new DomainResult(ResponseStatus.Error, errorMessage);
        }
    }
}

