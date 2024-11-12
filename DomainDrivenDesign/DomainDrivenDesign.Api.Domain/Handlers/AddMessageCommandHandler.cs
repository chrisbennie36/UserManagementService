using DomainDrivenDesign.Api.Data;
using DomainDrivenDesign.Api.Domain.Commands;
using MediatR;
using Serilog;

namespace DomainDrivenDesign.Api.Domain.Handlers;

public class AddMessageCommandHandler: IRequestHandler<AddMessageCommand, bool>
{
    private readonly AppDbContext appDbContext;

    public AddMessageCommandHandler(AppDbContext appDbContext)
    {
        this.appDbContext = appDbContext;
    }

    public async Task<bool> Handle(AddMessageCommand request, CancellationToken cancellationToken)
    {
        Log.Debug($"Handling {nameof(AddMessageCommand)}");

        try
        {
            appDbContext.Messages.Add(new Message { MessageText = "FirstTestMessage" });
            await appDbContext.SaveChangesAsync();
        }
        catch(Exception e)
        {
            Log.Error($"Error when writing a {nameof(Message)} to the database: {e.Message}");
            return false;
        }

        return true;
    }
}

