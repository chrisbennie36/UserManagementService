using UserManagementService.Api.Data;
using UserManagementService.Api.Domain.Commands;
using MediatR;
using Serilog;
using AutoMapper;
using UserManagementService.Api.Domain.Results;

namespace UserManagementService.Api.Domain.Handlers;

public class AddUserCommandHandler: IRequestHandler<AddUserCommand, UserResult?>
{
    private readonly AppDbContext appDbContext;
    private readonly IMapper mapper;

    public AddUserCommandHandler(AppDbContext appDbContext, IMapper mapper)
    {
        this.appDbContext = appDbContext;
        this.mapper = mapper;
    }

    public async Task<UserResult?> Handle(AddUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            User createdUser = appDbContext.Users.Add(new User 
            {
                Username = request.username,
                Password = request.password,
                Role = request.role,
                CreatedUtc = DateTime.UtcNow
            }).Entity;

            await appDbContext.SaveChangesAsync();

            if(createdUser == null)
            {
                return null;
            }

            return mapper.Map<UserResult>(createdUser);
        }
        catch(Exception e)
        {
            Log.Error($"Error when writing a {nameof(User)} to the database: {e.Message}");
            return null;
        }
    }
}

