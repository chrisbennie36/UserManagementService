using DomainDrivenDesign.Api.Data;
using DomainDrivenDesign.Api.Domain.Commands;
using MediatR;
using Serilog;

namespace DomainDrivenDesign.Api.Domain.Handlers;

public class AddUserCommandHandler: IRequestHandler<AddUserCommand, bool>
{
    private readonly AppDbContext appDbContext;

    public AddUserCommandHandler(AppDbContext appDbContext)
    {
        this.appDbContext = appDbContext;
    }

    public async Task<bool> Handle(AddUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            appDbContext.Users.Add(new User 
            {
                Username = request.username,
                Password = request.password,
                Role = request.role,
                CreatedUtc = DateTime.UtcNow
            });

            await appDbContext.SaveChangesAsync();
        }
        catch(Exception e)
        {
            Log.Error($"Error when writing a {nameof(User)} to the database: {e.Message}");
            return false;
        }

        return true;
    }
}

