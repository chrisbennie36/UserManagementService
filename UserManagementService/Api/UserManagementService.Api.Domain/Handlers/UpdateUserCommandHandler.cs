using UserManagementService.Api.Data;
using UserManagementService.Api.Domain.Commands;
using MediatR;
using Serilog;

namespace UserManagementService.Api.Domain.Handlers;

public class UpdateUserCommandHandler: IRequestHandler<UpdateUserCommand, bool>
{
    private readonly AppDbContext appDbContext;

    public UpdateUserCommandHandler(AppDbContext appDbContext)
    {
        this.appDbContext = appDbContext;
    }

    public async Task<bool> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            User? existingUser = await appDbContext.FindAsync<User>(request.userId);

            if(existingUser == null)
            {
                return false; //return NotFoundError?
            }

            existingUser.Username = request.username;
            existingUser.Password = request.password;
            existingUser.Role = request.role;
            existingUser.UpdatedUtc = DateTime.UtcNow;

            await appDbContext.SaveChangesAsync();
            return true;
        }
        catch(Exception e)
        {
            Log.Error($"Error when writing a {nameof(User)} to the database: {e.Message}");
            return false;
        }
    }
}

