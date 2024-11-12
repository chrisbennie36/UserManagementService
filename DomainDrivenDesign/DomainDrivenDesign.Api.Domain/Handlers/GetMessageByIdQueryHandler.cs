using AutoMapper;
using DomainDrivenDesign.Api.Data;
using DomainDrivenDesign.Api.Domain.Queries;
using DomainDrivenDesign.Api.Domain.Results;
using MediatR;
using Serilog;

namespace DomainDrivenDesign.Api.Domain.Handlers;

public class GetMessageByIdQueryHandler: IRequestHandler<GetMessageByIdQuery, MessageResult?>
{
    private readonly AppDbContext appDbContext;
    private readonly IMapper mapper;

    public GetMessageByIdQueryHandler(AppDbContext appDbContext, IMapper mapper)
    {
        this.appDbContext = appDbContext;
        this.mapper = mapper;
    }

    public async Task<MessageResult?> Handle(GetMessageByIdQuery request, CancellationToken cancellationToken)
    {
        try
        {
            Message? messageEntity = await appDbContext.Messages.FindAsync(request.messageId);
            
            if(messageEntity == null)
            {
                return null;
            }

            return mapper.Map<MessageResult>(messageEntity);
        }
        catch(Exception e)
        {
            Log.Error($"Error when retrieving a {nameof(Message)} from the database: {e.Message}");
            return null;
        }
    }
}

