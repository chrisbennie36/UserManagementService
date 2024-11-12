using AutoMapper;
using UserManagementService.Api.Data;
using UserManagementService.Api.Domain.Results;

namespace UserManagementService.Api.WebApplication.Mapper;

public class DefaultProfile : Profile
{
    public DefaultProfile()
    {
        CreateMap<User, UserResult>();
    }
}
