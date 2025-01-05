using UserManagementService.Api.Domain.Commands;
using UserManagementService.Api.Domain.Queries;
using UserManagementService.Api.WebApplication.Dtos;
using UserManagementService.Api.WebApplication.Extensions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using UserManagementService.Api.WebApplication.Responses;
using Serilog;
using Utilities.ResultPattern.Extensions;
using Utilities.ResultPattern;

namespace UserManagementService.Api.WebApplication.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly ISender sender;
    private readonly IValidator<UserDto> userDtoValidator;
    private readonly IMapper mapper;

    public UserController(ISender sender, IValidator<UserDto> userDtoValidator, IMapper mapper)
    {
        this.sender = sender;
        this.userDtoValidator = userDtoValidator;
        this.mapper = mapper;
    }

    [HttpGet("{userId}")]
    [ProducesResponseType<UserResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> GetUser([FromRoute] int userId)
    {        
        var result = await sender.Send(new GetUserByIdQuery(userId));

        return result.ToActionResult();
    }

    [HttpPost("/api/User/GetUserByCredentials")]
    [ProducesResponseType<UserResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)] 
    public async Task<ActionResult> GetUserByCredentials([FromBody] UserDto userDto)
    {
        if(string.IsNullOrWhiteSpace(userDto.Username) || string.IsNullOrWhiteSpace(userDto.Password)) {
            return NotFound();
        }

        var result = await sender.Send(new GetUserByUserCredentialsQuery(userDto.Username, userDto.Password));

        if(result.status == ResponseStatus.Success)
        {
            return Ok(mapper.Map<UserResponse>(result.resultModel));
        }

        return result.ToActionResult();
    }

    [HttpPost]
    [ProducesResponseType<UserResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> CreateUser(UserDto userDto)
    {        
        Log.Information("Creating a new user, validating first");
        
        //Can also validate automatically without the need to inject a validator. Requires a 3rd party library - see here: https://github.com/SharpGrip/FluentValidation.AutoValidation
        var validationResult = userDtoValidator.Validate(userDto);

        if(!validationResult.IsValid)
        {
            validationResult.AddToModelState(this.ModelState);
            return BadRequest(validationResult);
        }

        var result = await sender.Send(new AddUserCommand(userDto.Username, userDto.Password, userDto.Role));

        if(result.status == ResponseStatus.Success)
        {
            return Ok(mapper.Map<UserResponse>(result.resultModel));
        }

        return result.ToActionResult();
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

        return result.ToActionResult();
    }

    [HttpDelete("{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> DeleteUser([FromRoute] int userId)
    {        
        var result = await sender.Send(new DeleteUserCommand(userId));

        return result.ToActionResult();
    }
}
