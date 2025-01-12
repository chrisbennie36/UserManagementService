using Microsoft.EntityFrameworkCore;
using UserManagementService.Api.Data.Entities;

namespace UserManagementService.Api.Data.Repositories;

public class UserRepository : EfCoreEntityRepository<User>
{
    private readonly AppDbContext appDbContext;

    public UserRepository(AppDbContext appDbContext) : base(appDbContext)
    {
        this.appDbContext = appDbContext;
    }

    public async Task<User?> GetUserByCredentialsAsync(string username, string password)
    {
        return await appDbContext.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(u => u.Username == username && u.Password == password);
    }
}
