using UserManagementService.Api.Domain.Commands;
using MediatR;
using AutoMapper;
using UserManagementService.Api.Domain.Results;
using Utilities.ResultPattern;
using UserManagementService.Api.Data.Repositories;
using UserManagementService.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace UserManagementService.Api.Domain.Handlers;

public class AddUserCommandHandler: IRequestHandler<AddUserCommand, DomainResult<UserResult>>
{
    private readonly IMapper mapper;
    private IEntityRepository<User> userRepository;

    public AddUserCommandHandler(IMapper mapper, IEntityRepository<User> userRepository)
    {
        this.userRepository = userRepository;
        this.mapper = mapper;
    }

    public async Task<DomainResult<UserResult>> Handle(AddUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            User user = new User
            {
                Username = request.username,
                Password = request.password,
                CreatedUtc = DateTime.UtcNow
            };

            User createdUser = await userRepository.AddAsync(user);
            UserResult userResult = mapper.Map<UserResult>(createdUser);

            return new DomainResult<UserResult>(ResponseStatus.Success, userResult);

        }
        catch(DbUpdateException)
        {
            return new DomainResult<UserResult>(ResponseStatus.Error, null, $"Unable to add a {nameof(User)}, see the logs for more info");
        }
    }
}

