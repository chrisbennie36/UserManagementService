using UserManagementService.Api.Data;
using UserManagementService.Api.Domain.Commands;
using MediatR;
using Serilog;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace UserManagementService.Api.Domain.Handlers;

public class AddUserCommandHandler: IRequestHandler<AddUserCommand, bool>
{
    private readonly AppDbContext appDbContext;
    private readonly IMigrationsRepository migrationsRepository;

    public AddUserCommandHandler(AppDbContext appDbContext, IMigrationsRepository migrationsRepository)
    {
        this.appDbContext = appDbContext;
        this.migrationsRepository = migrationsRepository;
    }

    public async Task<bool> Handle(AddUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            await migrationsRepository.PerformMigrations();

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

