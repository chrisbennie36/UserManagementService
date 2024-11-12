using DomainDrivenDesign.Api.Domain.Commands;
using DomainDrivenDesign.Api.Domain.Queries;
using DomainDrivenDesign.Api.WebApplication.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DomainDrivenDesign.Api.WebApplication.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly ISender sender;
    private readonly ILogger<MessageController> logger;

    public UserController(ISender sender, ILogger<MessageController> logger)
    {
        this.sender = sender;
        this.logger = logger;
    }

    [HttpGet("{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetUser([FromRoute] int userId)
    {        
        var result = await sender.Send(new GetUserByIdQuery(userId));

        if(result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> CreateUser(UserDto userDto)
    {        
        var result = await sender.Send(new AddUserCommand(userDto.Username, userDto.Password, userDto.Role));

        if(result == false)
        {
            return BadRequest(result);
        }

        return Ok();
    }

    [HttpPut("{userId}")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> UpdateUser(int userId, UserDto userDto)
    {        
        var result = await sender.Send(new UpdateUserCommand(userId, userDto.Username, userDto.Password, userDto.Role));

        if(result == false)
        {
            return BadRequest(result);
        }

        return Ok();
    }

    [HttpDelete("{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> DeleteUser([FromRoute] int userId)
    {        
        var result = await sender.Send(new DeleteUserCommand(userId));

        if(result == false)
        {
            return BadRequest(result);
        }

        return Ok();
    }
}
