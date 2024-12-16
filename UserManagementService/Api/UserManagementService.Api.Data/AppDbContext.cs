using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Configuration;
using UserManagementService.Api.Data.Helpers;

namespace UserManagementService.Api.Data;

public class AppDbContext : DbContext
{
    protected readonly IConfiguration configuration;

    public AppDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        EntityTypeBuilder<User> userEntityBuilder = modelBuilder.Entity<User>();

         userEntityBuilder.Property(x => x.Password).HasConversion(v => EncryptionHelper.Encrypt(v), v => EncryptionHelper.Decrypt(v));
    }

    public DbSet<User> Users { get; set; }
}
