using FundRaising.Server.BLL.Interfaces.Repository;
using FundRaising.Server.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace FundRaising.Server.DAL.Repository;

public class GenericRepository<T>: IGenericRepository<T> 
    where T : BaseEntity
{
    private readonly AppDbContext _context;
    protected readonly DbSet<T> DbSet;

    protected GenericRepository(AppDbContext context)
    {
        _context = context;
        DbSet = context.Set<T>();
    }

    public async Task<List<T>> GetAllAsync()
    {
        return await DbSet.ToListAsync();
    }
    
    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await DbSet.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task AddAsync(T entity)
    {
        await _context.AddAsync(entity);
    }

    public void Update(T entity)
    {
        _context.Update(entity);
    }

    public void Delete(T entity)
    {
        _context.Remove(entity);
    }
    
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}