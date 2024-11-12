using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DomainDrivenDesign.Api.Data;

public class AppDbContext : DbContext
{
    protected readonly IConfiguration configuration;

    public AppDbContext(IConfiguration configuration) : base()
    {
        this.configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseNpgsql(configuration.GetConnectionString("ApiConnectionString"), b => b.MigrationsAssembly("DomainDrivenDesign.Api.WebApplication"));
    }

    public DbSet<Message> Messages { get; set; }
    public DbSet<User> Users { get; set; }
}
