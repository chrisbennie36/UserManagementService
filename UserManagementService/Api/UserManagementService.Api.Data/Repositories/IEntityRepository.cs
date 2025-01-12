using UserManagementService.Api.Data.Entities;

namespace UserManagementService.Api.Data.Repositories;

public interface IEntityRepository<T> where T : EntityBase
{
    public Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    public Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    public Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    public Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
