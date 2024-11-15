namespace UserManagementService.Api.Data;

public interface IMigrationsRepository
{
    Task PerformMigrations();
}
