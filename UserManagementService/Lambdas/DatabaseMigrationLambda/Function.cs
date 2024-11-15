using Amazon.Lambda.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UserManagementService.Api.Data;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace DatabaseMigrationLambda;

public class Function
{

    ServiceCollection serviceCollection;

    public Function()
    {
        ConfigureServices();
    }

    private void ConfigureServices()
    {
        serviceCollection = new ServiceCollection();

        serviceCollection.AddTransient<IConfiguration>();
        serviceCollection.AddDbContext<AppDbContext>();
        serviceCollection.AddTransient<IMigrationsRepository, MigrationsRepository>();
    }
    
    public void FunctionHandler(string input, ILambdaContext context)
    {
        using(ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider())
        {
            IMigrationsRepository? migrationsRepository = serviceProvider.GetService<IMigrationsRepository>();

            if(migrationsRepository == null)
            {
                context.Logger.LogError($"Could not retrieve {nameof(MigrationsRepository)} from the Service Provider");
                return;
            }
            
            try
            {
                migrationsRepository.PerformMigrations();
            }
            catch(Exception e)
            {
                context.Logger.LogError($"Error migrating the database: {e.Message}");
            }
        }
    }
}
