using AutoMapper;
using UserManagementService.Api.Data;
using UserManagementService.Api.Domain.Queries;
using UserManagementService.Api.Domain.Results;
using MediatR;
using Serilog;
using Utilities.ResultPattern;

namespace UserManagementService.Api.Domain.Handlers;

public class GetUserByIdQueryHandler: IRequestHandler<GetUserByIdQuery, DomainResult<UserResult>>
{
    private readonly AppDbContext appDbContext;
    private readonly IMapper mapper;

    public GetUserByIdQueryHandler(AppDbContext appDbContext, IMapper mapper)
    {
        this.appDbContext = appDbContext;
        this.mapper = mapper;
    }

    public async Task<DomainResult<UserResult>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            User? existingUser = await appDbContext.Users.FindAsync(request.userId);
            
            if(existingUser == null)
            {
                Log.Error($"Could not find a {nameof(User)} with ID {request.userId}");
                return new DomainResult<UserResult>(ResponseStatus.NotFound, null);
            }

            UserResult userResult = mapper.Map<UserResult>(existingUser);
            return new DomainResult<UserResult>(ResponseStatus.Success, userResult);
        }
        catch(Exception e)
        {
            Log.Error($"Error when retrieving a {nameof(User)} with ID {request.userId} from the database: {e.Message}");
            return new DomainResult<UserResult>(ResponseStatus.Error, null, $"Error when retrieving a {nameof(User)} from the database");
        }
    }
}

