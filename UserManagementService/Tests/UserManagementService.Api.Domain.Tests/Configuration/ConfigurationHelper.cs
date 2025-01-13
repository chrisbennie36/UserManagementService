using Microsoft.Extensions.Configuration;

namespace UserManagementService.Api.Domain.Tests.Configuration;

public static class ConfigurationHelper
{
     public static IConfiguration InitConfiguration()
         {
             var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.Test.json")
                 .AddEnvironmentVariables() 
                 .Build();
                 return config;
         }
}
