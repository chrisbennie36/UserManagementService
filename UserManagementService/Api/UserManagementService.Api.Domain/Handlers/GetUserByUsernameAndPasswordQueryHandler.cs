using AutoMapper;
using UserManagementService.Api.Data;
using UserManagementService.Api.Domain.Queries;
using UserManagementService.Api.Domain.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace UserManagementService.Api.Domain.Handlers;

public class GetUserByUsernameAndPasswordQueryHandler: IRequestHandler<GetUserByUsernameAndPasswordQuery, UserResult?>
{
    private readonly AppDbContext appDbContext;
    private readonly IMapper mapper;

    public GetUserByUsernameAndPasswordQueryHandler(AppDbContext appDbContext, IMapper mapper)
    {
        this.appDbContext = appDbContext;
        this.mapper = mapper;
    }

    public async Task<UserResult?> Handle(GetUserByUsernameAndPasswordQuery request, CancellationToken cancellationToken)
    {
        try
        {
            User? existingUser = await appDbContext.Users.SingleOrDefaultAsync(u => u.Username == request.username && u.Password == request.password);
            
            if(existingUser == null)
            {
                return null;
            }

            return mapper.Map<UserResult>(existingUser);
        }
        catch(Exception e)
        {
            Log.Error($"Error when retrieving a {nameof(User)} from the database: {e.Message}");
            return null;
        }
    }
}

