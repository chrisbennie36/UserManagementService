
using Microsoft.EntityFrameworkCore;

namespace UserManagementService.Api.Data;

public class MigrationsRepository : IMigrationsRepository
{
    private readonly AppDbContext appDbContext;
    
    public MigrationsRepository(AppDbContext appDbContext)
    {
        this.appDbContext = appDbContext;
    }

    public async Task PerformMigrations()
    {
        try
        {
            await appDbContext.Database.MigrateAsync();
        }
        catch(Exception)
        {
            throw;
        }
    }

}
