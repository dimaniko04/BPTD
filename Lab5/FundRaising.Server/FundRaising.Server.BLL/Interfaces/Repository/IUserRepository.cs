using FundRaising.Server.Core.Entities;

namespace FundRaising.Server.BLL.Interfaces.Repository;

public interface IUserRepository
{
    Task<User?> GetUserByEmailAsync(string email);
    Task AddAsync(User user);
    Task SaveChangesAsync();
}