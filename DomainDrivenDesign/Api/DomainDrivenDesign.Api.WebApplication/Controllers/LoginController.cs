using System.IdentityModel.Tokens.Jwt;
using System.Text;
using DomainDrivenDesign.Api.Domain.Queries;
using DomainDrivenDesign.Api.Domain.Results;
using DomainDrivenDesign.Api.WebApplication.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace DomainDrivenDesign.Api.WebApplication.Controllers;

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
        UserResult? existingUser = await sender.Send(new GetUserByUsernameAndPasswordQuery(user.Username, user.Password));

        if(existingUser == null)
        {
            return null;
        }

        return existingUser;
    }

    private string GenerateJwtToken()
    {        
        /*SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"] ?? string.Empty));
        SigningCredentials credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);*/

        /*var claims = new[] {
        new Claim(JwtRegisteredClaimNames.Sub, userInfo.Username),
        new Claim(JwtRegisteredClaimNames.Email, userInfo.EmailAddress),
        new Claim("DateOfJoing", userInfo.DateOfJoing.ToString("yyyy-MM-dd")),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())*/

        /*JwtSecurityToken token = new JwtSecurityToken(
            configuration["Jwt:Issuer"] ?? string.Empty,
            configuration["Jwt:Issuer"] ?? string.Empty,
            //claims,
            null,
            expires: DateTime.Now.AddMinutes(120),
            signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);*/

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
