using System.Collections.ObjectModel;

namespace FundRaising.Server.BLL.Interfaces.Repository;

public interface IGenericRepository<T>
{
    Task<List<T>> GetAllAsync();
    Task<T?> GetByIdAsync(Guid id);
    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
    Task SaveChangesAsync();
}