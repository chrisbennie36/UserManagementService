using UserManagementService.Api.Data;
using UserManagementService.Api.Domain.Commands;
using MediatR;
using Serilog;
using AutoMapper;
using UserManagementService.Api.Domain.Results;
using Utilities.ResultPattern;

namespace UserManagementService.Api.Domain.Handlers;

public class AddUserCommandHandler: IRequestHandler<AddUserCommand, DomainResult<UserResult>>
{
    private readonly AppDbContext appDbContext;
    private readonly IMapper mapper;

    public AddUserCommandHandler(AppDbContext appDbContext, IMapper mapper)
    {
        this.appDbContext = appDbContext;
        this.mapper = mapper;
    }

    public async Task<DomainResult<UserResult>> Handle(AddUserCommand request, CancellationToken cancellationToken)
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
                return new DomainResult<UserResult>(ResponseStatus.Error, null, $"Could not create a {nameof(User)} in the database");
            }

            UserResult userResult = mapper.Map<UserResult>(createdUser);

            return new DomainResult<UserResult>(ResponseStatus.Success, userResult);

        }
        catch(Exception e)
        {
            string errorMessage = $"Error when writing a {nameof(User)} to the database: {e.Message}";
            Log.Error(errorMessage);
            return new DomainResult<UserResult>(ResponseStatus.Error, null, errorMessage);
        }
    }
}

