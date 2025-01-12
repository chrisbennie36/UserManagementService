using AutoMapper;
using UserManagementService.Api.Data.Entities;
using UserManagementService.Api.Domain.Results;
using UserManagementService.Api.WebApplication.Responses;

namespace UserManagementService.Api.WebApplication.Mapper;

public class DefaultProfile : Profile
{
    public DefaultProfile()
    {
        CreateMap<User, UserResult>();
        CreateMap<UserResult, UserResponse>();
    }
}
