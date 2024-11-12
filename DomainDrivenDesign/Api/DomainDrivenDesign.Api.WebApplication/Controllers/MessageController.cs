using DomainDrivenDesign.Api.Domain.Commands;
using DomainDrivenDesign.Api.Domain.Queries;
using DomainDrivenDesign.Api.WebApplication.Dtos;
using DomainDrivenDesign.Api.WebApplication.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DomainDrivenDesign.Api.WebApplication.Controllers;

[ApiController]
[Route("api/Messages")]
public class MessageController : ControllerBase
{
    private readonly ISender sender;
    private readonly ILogger<MessageController> logger;
    private readonly IValidator<MessageDto> messageDtoValidator;

    public MessageController(ISender sender, ILogger<MessageController> logger, IValidator<MessageDto> messageDtoValidator)
    {
        this.sender = sender;
        this.logger = logger;
        this.messageDtoValidator = messageDtoValidator;
    }

    [HttpGet("{messageId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetMessage([FromRoute] int messageId)
    {        
        var result = await sender.Send(new GetMessageByIdQuery(messageId));

        if(result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> SendMessage(MessageDto messageDto)
    {        
        //Can also validate automatically without the need to inject a validator. Requires a 3rd party library - see here: https://github.com/SharpGrip/FluentValidation.AutoValidation
        var validationResult = messageDtoValidator.Validate(messageDto);

        if(!validationResult.IsValid)
        {
            validationResult.AddToModelState(this.ModelState);
            return BadRequest(validationResult);
        }

        var result = await sender.Send(new AddMessageCommand(messageDto.MessageText));

        if(result == false)
        {
            return BadRequest(result);
        }

        return Ok();
    }

    [HttpPost("{messageId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> UpdateMessage([FromRoute] int messageId, MessageDto messageDto)
    {        
        //Can also validate automatically without the need to inject a validator. Requires a 3rd party library - see here: https://github.com/SharpGrip/FluentValidation.AutoValidation
        var validationResult = messageDtoValidator.Validate(messageDto);

        if(!validationResult.IsValid)
        {
            validationResult.AddToModelState(this.ModelState);
            return BadRequest(validationResult);
        }

        var result = await sender.Send(new UpdateMessageCommand(messageId, messageDto.MessageText));

        if(result == false)
        {
            return BadRequest(result);
        }

        return Ok();
    }

    [HttpDelete("{messageId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> DeleteMessage([FromRoute] int messageId)
    {        
        var result = await sender.Send(new DeleteMessageCommand(messageId));

        if(result == false)
        {
            return BadRequest(result);
        }

        return Ok();
    }
}
