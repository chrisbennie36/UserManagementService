using System.Text;
using UserManagementService.Api.Data;
using UserManagementService.Api.Domain.Commands;
using UserManagementService.Api.WebApplication.Validation;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using NSwag.Generation.Processors.Security;
using UserManagementService.Api.Data.Helpers;
using UserManagementService.Api.WebApplication.ExceptionHandler;
using Amazon.CloudWatchLogs;
using Amazon.Runtime;
using Serilog.Sinks.AwsCloudWatch;
using Amazon;
using Utilities.ConfigurationManager.Extensions;
using UserManagementService.Api.Data.Repositories;
using UserManagementService.Api.Data.Entities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddMvcCore().AddApiExplorer();
//Nswag: Useful setup link for both NSwag and Swashbuckle here: https://code-maze.com/aspnetcore-swashbuckle-vs-nswag/
builder.Services.AddOpenApiDocument(config => 
{
    config.Title = "User Management Service";
    config.AddSecurity("Bearer", Enumerable.Empty<string>(), new NSwag.OpenApiSecurityScheme
    {
        Type = NSwag.OpenApiSecuritySchemeType.Http,
        In = NSwag.OpenApiSecurityApiKeyLocation.Header,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        BearerFormat = "JWT"
        
    });
    config.OperationProcessors.Add(new OperationSecurityScopeProcessor("Bearer"));
});

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(AddUserCommand).Assembly));
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddValidatorsFromAssemblyContaining<UserDtoValidator>();

builder.Services.AddDbContext<AppDbContext>();
builder.Services.AddTransient<IMigrationsRepository, MigrationsRepository>();
builder.Services.AddTransient<IEntityRepository<User>, UserRepository>();
builder.Services.AddTransient<UserRepository>();    //For concrete class constructor injection 

builder.Services.AddProblemDetails().AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => 
{
    options.RequireHttpsMetadata = false;   //NOTE: ONLY FOR DEVELOPMENT
    options.Authority = "localhost:5175";
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration.GetStringValue("Jwt:Key")))
    };
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.LoginPath = "/Account/Login";
})
.AddOpenIdConnect(options =>
{
    options.Authority = $"{builder.Configuration.GetStringValue("Keycloak:BaseUrl")}/auth/realms/{builder.Configuration.GetStringValue("Keycloak:Realm")}";
    options.ClientId = builder.Configuration.GetStringValue("Keycloak:ClientId");
    options.ClientSecret = builder.Configuration.GetStringValue("Keycloak:ClientSecret");
    options.ResponseType = "code";
    options.SaveTokens = true;
    options.Scope.Add("openid");
    options.CallbackPath = "api/user/login-callback"; // Update callback path
    //options.SignedOutCallbackPath = "/logout-callback"; // Update signout callback path
    options.TokenValidationParameters = new TokenValidationParameters
    {
        NameClaimType = "preferred_username",
        RoleClaimType = "roles"
    };
});

EncryptionHelper.SetEncryptionKey(builder.Configuration.GetStringValue("Encryption:Key"));
EncryptionHelper.SetInitialisationVector(builder.Configuration.GetStringValue("Encryption:IV"));

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

var app = builder.Build();

if(builder.Configuration.GetBoolValue("AwsCloudwatchLogging:Enabled") == true)
{
    var client = new AmazonCloudWatchLogsClient(new BasicAWSCredentials(builder.Configuration.GetStringValue("AwsCloudwatchLogging:AccessKey"), builder.Configuration.GetStringValue("AwsCloudwatchLogging:SecretKey")), RegionEndpoint.USEast1);

    Log.Logger = new LoggerConfiguration().WriteTo.AmazonCloudWatch(
        logGroup: builder.Configuration.GetStringValue("AwsCloudwatchLogging:LogGroup"),
        logStreamPrefix: builder.Configuration.GetStringValue("AwsCloudwatchLogging:LogStreamPrefix"),
        restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Verbose,
        createLogGroup: true,
        appendUniqueInstanceGuid: true,
        appendHostName: false,
        logGroupRetentionPolicy: LogGroupRetentionPolicy.ThreeDays,
        cloudWatchClient: client).CreateLogger();
}
else
{
    Log.Logger = new LoggerConfiguration().WriteTo.File($"{builder.Configuration.GetStringValue("AwsCloudwatchLogging:Location")}/logs-", rollingInterval: RollingInterval.Day).MinimumLevel.Debug().CreateLogger();
}

if(app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi();
}

app.UseStatusCodePages();
app.UseExceptionHandler();

//The following three should be in this exact order - see here: https://stackoverflow.com/questions/43574552/authorization-in-asp-net-core-always-401-unauthorized-for-authorize-attribute
app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();

app.Map("/api/user/login-callback", callback => 
{
    callback.Run(async context => 
    {
        await context.Response.WriteAsync("Authentication Successful");
    });
});

/*app.Map("/logout-callback", callback => 
{
    callback.Run(async context => 
    {
        await context.Response.WriteAsync("Sign Out Successful");
    });
});*/

app.MapControllers();

app.UseEndpoints(endpoints => 
{
    endpoints.MapControllerRoute(
        name: "login-callback",
        pattern: "login-callback",
        defaults: new { controller = "User", action = "LoginCallback"}
    );
});

app.Run();
