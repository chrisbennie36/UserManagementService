using UserManagementService.Api.Data;
using UserManagementService.Api.Domain.Commands;
using MediatR;
using Serilog;
using UserManagementService.Api.Domain.Results;

namespace UserManagementService.Api.Domain.Handlers;

public class DeleteUserCommandHandler: IRequestHandler<DeleteUserCommand, DomainResult>
{
    private readonly AppDbContext appDbContext;

    public DeleteUserCommandHandler(AppDbContext appDbContext)
    {
        this.appDbContext = appDbContext;
    }

    public async Task<DomainResult> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            //Note that this results in two operations, a GET and a DELETE. It's also possible to create a stub Message object with just the ID and then call the context.Remove method which will
            //only trigger one DELETE operation: https://www.learnentityframeworkcore.com/dbcontext/deleting-data
            appDbContext.Remove(appDbContext.Users.Single(m => m.Id == request.userId));
            await appDbContext.SaveChangesAsync();
        }
        catch(Exception e)
        {
            Log.Error($"Error when deleting a {nameof(User)} with ID {request.userId} from the database: {e.Message}");
            return new DomainResult(ResponseStatus.Error, $"Error when deleting a {nameof(User)} from the database");
        }

        return new DomainResult(ResponseStatus.Success);
    }
}

