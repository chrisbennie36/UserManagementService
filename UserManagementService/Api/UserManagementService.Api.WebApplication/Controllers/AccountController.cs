using System.IdentityModel.Tokens.Jwt;
using System.Text;
using UserManagementService.Api.Domain.Queries;
using UserManagementService.Api.Domain.Results;
using UserManagementService.Api.WebApplication.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace UserManagementService.Api.WebApplication.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly ISender sender;
    private readonly IConfiguration configuration;
    private readonly ILogger<AccountController> logger;

    public AccountController(ISender sender, IConfiguration configuration, ILogger<AccountController> logger)
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

    public async Task<IActionResult> LoginCallback()
    {
        var authResult = await HttpContext.AuthenticateAsync(JwtBearerDefaults.AuthenticationScheme);
        if (authResult?.Succeeded != true)
        {
            // Handle failed authentication
            return RedirectToAction("Login");
        }

        // Get the access token and refresh token
        var accessToken = authResult.Properties?.GetTokenValue("access_token");
        var refreshToken = authResult.Properties?.GetTokenValue("refresh_token");

        if(accessToken == null)
        {
            return RedirectToAction("Login");
        }

        // Save the tokens to the user's session or database
        HttpContext.Session.SetString("access_token", accessToken);

        if(!string.IsNullOrWhiteSpace(refreshToken))
        {
            HttpContext.Session.SetString("refresh_token", refreshToken);
        }

        // Redirect the user to the desired page
        return RedirectToAction(actionName: "GetUser", controllerName: "User");
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
