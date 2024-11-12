using AutoMapper;
using UserManagementService.Api.Data;
using UserManagementService.Api.Domain.Queries;
using UserManagementService.Api.Domain.Results;
using MediatR;
using Serilog;

namespace UserManagementService.Api.Domain.Handlers;

public class GetUserByIdQueryHandler: IRequestHandler<GetUserByIdQuery, UserResult?>
{
    private readonly AppDbContext appDbContext;
    private readonly IMapper mapper;

    public GetUserByIdQueryHandler(AppDbContext appDbContext, IMapper mapper)
    {
        this.appDbContext = appDbContext;
        this.mapper = mapper;
    }

    public async Task<UserResult?> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            User? existingUser = await appDbContext.Users.FindAsync(request.userId);
            
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

