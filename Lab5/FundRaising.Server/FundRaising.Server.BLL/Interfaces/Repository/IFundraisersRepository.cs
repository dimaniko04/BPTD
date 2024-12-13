using FundRaising.Server.Core.Entities;

namespace FundRaising.Server.BLL.Interfaces.Repository;

public interface IFundraisersRepository
    : IGenericRepository<Fundraiser>
{
    Task<List<Fundraiser>> GetAllUserFundraisers(Guid userId);
    Task<bool> IsUserFundraiser(Guid userId, Guid id);
}