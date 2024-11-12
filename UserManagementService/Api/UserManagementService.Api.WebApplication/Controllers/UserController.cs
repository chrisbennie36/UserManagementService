using UserManagementService.Api.Domain.Commands;
using UserManagementService.Api.Domain.Queries;
using UserManagementService.Api.WebApplication.Dtos;
using UserManagementService.Api.WebApplication.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace UserManagementService.Api.WebApplication.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly ISender sender;
    private readonly IValidator<UserDto> userDtoValidator;

    public UserController(ISender sender, IValidator<UserDto> userDtoValidator)
    {
        this.sender = sender;
        this.userDtoValidator = userDtoValidator;
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
        //Can also validate automatically without the need to inject a validator. Requires a 3rd party library - see here: https://github.com/SharpGrip/FluentValidation.AutoValidation
        var validationResult = userDtoValidator.Validate(userDto);

        if(!validationResult.IsValid)
        {
            validationResult.AddToModelState(this.ModelState);
            return BadRequest(validationResult);
        }

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
        //Can also validate automatically without the need to inject a validator. Requires a 3rd party library - see here: https://github.com/SharpGrip/FluentValidation.AutoValidation
        var validationResult = userDtoValidator.Validate(userDto);

        if(!validationResult.IsValid)
        {
            validationResult.AddToModelState(this.ModelState);
            return BadRequest(validationResult);
        }

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
