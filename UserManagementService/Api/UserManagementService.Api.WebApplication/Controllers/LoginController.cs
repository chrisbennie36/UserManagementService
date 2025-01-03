using System.IdentityModel.Tokens.Jwt;
using System.Text;
using UserManagementService.Api.Domain.Queries;
using UserManagementService.Api.Domain.Results;
using UserManagementService.Api.WebApplication.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace UserManagementService.Api.WebApplication.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoginController : ControllerBase
{
    private readonly ISender sender;
    private readonly IConfiguration configuration;
    private readonly ILogger<LoginController> logger;

    public LoginController(ISender sender, IConfiguration configuration, ILogger<LoginController> logger)
    {
        this.sender = sender;
        this.configuration = configuration;
        this.logger = logger;
    }

    [AllowAnonymous]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> Login([FromBody] UserDto userDto)
    {        
        logger.LogInformation("Attempting login for user {username}", userDto.Username);

        UserResult? authenticatedUser = await AuthenticateUser(userDto);

        if(authenticatedUser == null)
        {
            return Unauthorized();
        }

        string jwtToken = GenerateJwtToken();

        return Ok(jwtToken);
    }

    private async Task<UserResult?> AuthenticateUser(UserDto user)
    {
        var result = await sender.Send(new GetUserByUserCredentialsQuery(user.Username, user.Password));

        return result.resultModel;
    }

    private string GenerateJwtToken()
    {        
        var handler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(configuration["Jwt:Key"]);
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(key),
            SecurityAlgorithms.HmacSha256Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            //Subject = GenerateClaims(user),
            Expires = DateTime.UtcNow.AddMinutes(15),
            SigningCredentials = credentials,
        };

        var token = handler.CreateToken(tokenDescriptor);
        return handler.WriteToken(token);
    }
}
