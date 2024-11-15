using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace UserManagementService.Api.Data;

public class AppDbContext : DbContext
{
    protected readonly IConfiguration configuration;
    private readonly string connectionString;

    public AppDbContext(IConfiguration configuration) : base()
    {
        this.configuration = configuration;
    }

    public AppDbContext(string connectionString) : base()
    {
        this.connectionString = connectionString;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        if(string.IsNullOrWhiteSpace(connectionString))
        {
            options.UseNpgsql(configuration.GetConnectionString("ApiAwsConnectionString"), b => b.MigrationsAssembly("UserManagementService.Api.WebApplication"));
        }
        else
        {
            options.UseNpgsql(connectionString, b => b.MigrationsAssembly("UserManagementService.Api.WebApplication"));
        }
    }

    public DbSet<User> Users { get; set; }
}
