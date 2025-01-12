using AutoMapper;
using UserManagementService.Api.Data;
using UserManagementService.Api.Domain.Queries;
using UserManagementService.Api.Domain.Results;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Utilities.ResultPattern;
using UserManagementService.Api.Data.Entities;
using UserManagementService.Api.Data.Repositories;

namespace UserManagementService.Api.Domain.Handlers;

public class GetUserByUserCredentialsQueryHandler: IRequestHandler<GetUserByUserCredentialsQuery, DomainResult<UserResult>>
{
    private readonly UserRepository userRepository;
    private readonly IMapper mapper;

    public GetUserByUserCredentialsQueryHandler(UserRepository userRepository, IMapper mapper)
    {
        this.userRepository = userRepository;
        this.mapper = mapper;
    }

    public async Task<DomainResult<UserResult>> Handle(GetUserByUserCredentialsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            User? existingUser = await userRepository.GetUserByCredentialsAsync(request.username, request.password);
            
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

