using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Configuration;
using UserManagementService.Api.Data.Entities;
using UserManagementService.Api.Data.Helpers;

namespace UserManagementService.Api.Data;

public class AppDbContext : DbContext
{
    protected readonly IConfiguration configuration;

    public AppDbContext(IConfiguration configuration, DbContextOptions<AppDbContext> options) : base(options)
    {
        this.configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
       // options.UseNpgsql(configuration.GetConnectionString("ApiAwsConnectionString"), b => b.MigrationsAssembly("RecipeRandomizer.Api.WebApplication"));
        options.UseNpgsql(configuration.GetConnectionString("ApiLocalConnectionString"), b => b.MigrationsAssembly("UserManagementService.Api.WebApplication"));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        EntityTypeBuilder<User> userEntityBuilder = modelBuilder.Entity<User>();

         userEntityBuilder.Property(x => x.Password).HasConversion(v => EncryptionHelper.Encrypt(v), v => EncryptionHelper.Decrypt(v));
    }

    public DbSet<User> Users { get; set; }
}
