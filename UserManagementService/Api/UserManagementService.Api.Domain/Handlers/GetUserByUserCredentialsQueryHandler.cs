using AutoMapper;
using UserManagementService.Api.Data;
using UserManagementService.Api.Domain.Queries;
using UserManagementService.Api.Domain.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Utilities.ResultPattern;

namespace UserManagementService.Api.Domain.Handlers;

public class GetUserByUserCredentialsQueryHandler: IRequestHandler<GetUserByUserCredentialsQuery, DomainResult<UserResult>>
{
    private readonly AppDbContext appDbContext;
    private readonly IMapper mapper;

    public GetUserByUserCredentialsQueryHandler(AppDbContext appDbContext, IMapper mapper)
    {
        this.appDbContext = appDbContext;
        this.mapper = mapper;
    }

    public async Task<DomainResult<UserResult>> Handle(GetUserByUserCredentialsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            User? existingUser = await appDbContext.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(u => u.Username == request.username && u.Password == request.password);
            
            if(existingUser == null)
            {
                string errorMessage = $"Could not retrieve a {nameof(User)} for the provided credentials from the database";
                Log.Error(errorMessage);
                return new DomainResult<UserResult>(ResponseStatus.NotFound, null, errorMessage);
            }

            UserResult userResult = mapper.Map<UserResult>(existingUser);
            return new DomainResult<UserResult>(ResponseStatus.Success, userResult);
        }
        catch(Exception e)
        {
            string errorMessage = $"Error when retrieving a {nameof(User)} using {nameof(User)} credentials from the database: {e.Message}";
            Log.Error(errorMessage);
            return new DomainResult<UserResult>(ResponseStatus.Error, null, errorMessage);
        }
    }
}

