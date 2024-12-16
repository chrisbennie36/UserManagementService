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
using Microsoft.AspNetCore.Identity.Data;
using Amazon;
using Microsoft.EntityFrameworkCore;

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

builder.Services.AddScoped<AppDbContext>(db => new AppDbContext(new DbContextOptionsBuilder().UseNpgsql(builder.Configuration.GetConnectionString("ApiLocalConnectionString")).Options));
builder.Services.AddTransient<IMigrationsRepository, MigrationsRepository>();

builder.Services.AddProblemDetails().AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => 
{
    options.RequireHttpsMetadata = false;   //NOTE: ONLY FOR DEVELOPMENT
    options.Authority = "localhost:5175";
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"] ?? string.Empty))
    };
});

EncryptionHelper.SetEncryptionKey(builder.Configuration["Encryption:Key"] ?? string.Empty);
EncryptionHelper.SetInitialisationVector(builder.Configuration["Encryption:IV"] ?? string.Empty);

builder.Services.AddAuthorization();

var app = builder.Build();

if(Boolean.Parse(builder.Configuration["AwsCloudwatchLogging:Enabled"]) == true)
{
    var client = new AmazonCloudWatchLogsClient(new BasicAWSCredentials(builder.Configuration["AwsCloudwatchLogging:AccessKey"], builder.Configuration["AwsCloudwatchLogging:SecretKey"]), RegionEndpoint.USEast1);

    Log.Logger = new LoggerConfiguration().WriteTo.AmazonCloudWatch(
        logGroup: builder.Configuration["AwsCloudwatchLogging:LogGroup"],
        logStreamPrefix: builder.Configuration["AwsCloudwatchLogging:LogStreamPrefix"],
        restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Verbose,
        createLogGroup: true,
        appendUniqueInstanceGuid: true,
        appendHostName: false,
        logGroupRetentionPolicy: LogGroupRetentionPolicy.ThreeDays,
        cloudWatchClient: client).CreateLogger();
}
else
{
    Log.Logger = new LoggerConfiguration().WriteTo.File("./Logs/logs-", rollingInterval: RollingInterval.Day).MinimumLevel.Debug().CreateLogger();
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

app.MapControllers();

app.Run();
