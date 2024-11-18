using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Configuration;
using UserManagementService.Api.Data.Converters;
using UserManagementService.Api.Data.Helpers;

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
            options.UseNpgsql(configuration.GetConnectionString("ApiLocalConnectionString"), b => b.MigrationsAssembly("UserManagementService.Api.WebApplication"));
        }
        else
        {
            options.UseNpgsql(connectionString, b => b.MigrationsAssembly("UserManagementService.Api.WebApplication"));
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        EntityTypeBuilder<User> userEntityBuilder = modelBuilder.Entity<User>();

         userEntityBuilder.Property(x => x.Password).HasConversion(v => EncryptionHelper.Encrypt(v), v => EncryptionHelper.Decrypt(v));
    }

    public DbSet<User> Users { get; set; }
}
