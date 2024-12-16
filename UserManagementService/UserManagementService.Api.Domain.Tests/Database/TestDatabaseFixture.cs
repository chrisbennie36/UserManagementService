using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using UserManagementService.Api.Data;
using UserManagementService.Api.Domain.Tests.Configuration;

namespace UserManagementService.Api.Domain.Tests.Database;

public class TestDatabaseFixture
{
    private string ConnectionString = ConfigurationHelper.InitConfiguration().GetConnectionString("ApiLocalConnectionString");


    private static readonly object _lock = new();
    private static bool databaseInitialized;

    public TestDatabaseFixture()
    {
        lock (_lock)
        {
            if (!databaseInitialized)
            {
                using (var context = CreateContext())
                {
                    context.Database.EnsureDeleted();
                    context.Database.EnsureCreated();

                    context.SaveChanges();
                }

                databaseInitialized = true;
            }
        }
    }

     public AppDbContext CreateContext()
     {
        return new AppDbContext(new DbContextOptionsBuilder<AppDbContext>().UseNpgsql(ConnectionString).Options);
     }
}
