using DomainDrivenDesign.Api.Data;
using DomainDrivenDesign.Api.Domain.Commands;
using MediatR;
using Serilog;

namespace DomainDrivenDesign.Api.Domain.Handlers;

public class DeleteMessageCommandHandler: IRequestHandler<DeleteMessageCommand, bool>
{
    private readonly AppDbContext appDbContext;

    public DeleteMessageCommandHandler(AppDbContext appDbContext)
    {
        this.appDbContext = appDbContext;
    }

    public async Task<bool> Handle(DeleteMessageCommand request, CancellationToken cancellationToken)
    {
        try
        {
            //Note that this results in two operations, a GET and a DELETE. It's also possible to create a stub Message object with just the ID and then call the context.Remove method which will
            //only trigger one DELETE operation: https://www.learnentityframeworkcore.com/dbcontext/deleting-data
            appDbContext.Remove(appDbContext.Messages.Single(m => m.Id == request.messageId));
            await appDbContext.SaveChangesAsync();
        }
        catch(Exception e)
        {
            Log.Error($"Error when deleting a {nameof(Message)} from the database: {e.Message}");
            return false;
        }

        return true;
    }
}

