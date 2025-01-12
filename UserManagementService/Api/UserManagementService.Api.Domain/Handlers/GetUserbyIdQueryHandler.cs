using AutoMapper;
using UserManagementService.Api.Data;
using UserManagementService.Api.Domain.Queries;
using UserManagementService.Api.Domain.Results;
using MediatR;
using Serilog;
using Utilities.ResultPattern;
using UserManagementService.Api.Data.Entities;
using UserManagementService.Api.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace UserManagementService.Api.Domain.Handlers;

public class GetUserByIdQueryHandler: IRequestHandler<GetUserByIdQuery, DomainResult<UserResult>>
{
    private readonly IEntityRepository<User> userRepository;
    private readonly IMapper mapper;

    public GetUserByIdQueryHandler(IEntityRepository<User> userRepository, IMapper mapper)
    {
        this.userRepository = userRepository;
        this.mapper = mapper;
    }

    public async Task<DomainResult<UserResult>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            User? existingUser = await userRepository.GetByIdAsync(request.userId);
            
            if(existingUser == null)
            {
                Log.Error($"Could not find a {nameof(User)} with ID {request.userId} in the database");
                return new DomainResult<UserResult>(ResponseStatus.NotFound, null);
            }

            UserResult userResult = mapper.Map<UserResult>(existingUser);
            return new DomainResult<UserResult>(ResponseStatus.Success, userResult);
        }
        catch(DbUpdateException e)
        {
            Log.Error($"Error when retrieving a {nameof(User)} with ID {request.userId} from the database: {e.InnerException?.Message}");
            return new DomainResult<UserResult>(ResponseStatus.Error, null, $"Error when retrieving a {nameof(User)} from the database");
        }
    }
}

