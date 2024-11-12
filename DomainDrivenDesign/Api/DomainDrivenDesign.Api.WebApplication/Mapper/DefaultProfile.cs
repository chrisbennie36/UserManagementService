using AutoMapper;
using DomainDrivenDesign.Api.Data;
using DomainDrivenDesign.Api.Domain.Results;

namespace DomainDrivenDesign.Api.WebApplication.Mapper;

public class DefaultProfile : Profile
{
    public DefaultProfile()
    {
        CreateMap<Message, MessageResult>();
        CreateMap<User, UserResult>();
    }
}
