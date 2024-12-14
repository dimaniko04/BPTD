using FundRaising.Server.BLL.DTOs.Fundraiser;
using FundRaising.Server.Core.Entities;

namespace FundRaising.Server.BLL.Services.Fundraisers;

public interface IFundraisersService
{
    Task<IEnumerable<FundraiserDto>> 
        GetAllUserFundraisers(Guid userId);
    Task<FundraiserDto> GetFundraiser(Guid id);
    Task AddFundraiser(Guid userId, CreateFundraiserDto fundraiser);
    Task UpdateFundraiser(
        Guid userId,
        Guid id,
        UpdateFundraiserDto fundraiser);
    Task DeleteFundraiser(Guid userId, Guid id);
    Task Donate(Guid userId, Guid id, PaymentDto paymentDto);
}