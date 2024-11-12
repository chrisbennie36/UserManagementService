using DomainDrivenDesign.Api.Data;
using DomainDrivenDesign.Api.Domain.Commands;
using MediatR;
using Serilog;

namespace DomainDrivenDesign.Api.Domain.Handlers;

public class UpdateMessageCommandHandler: IRequestHandler<UpdateMessageCommand, bool>
{
    private readonly AppDbContext appDbContext;

    public UpdateMessageCommandHandler(AppDbContext appDbContext)
    {
        this.appDbContext = appDbContext;
    }

    public async Task<bool> Handle(UpdateMessageCommand request, CancellationToken cancellationToken)
    {
        try
        {
            Message? messageEntity = await appDbContext.FindAsync<Message>(request.messageId);

            if(messageEntity == null)
            {
                return false; //return NotFoundError?
            }

            messageEntity.MessageText = request.messageText;
            messageEntity.UpdatedUtc = DateTime.UtcNow;

            await appDbContext.SaveChangesAsync();
            return true;
        }
        catch(Exception e)
        {
            Log.Error($"Error when writing a {nameof(Message)} to the database: {e.Message}");
            return false;
        }
    }
}

