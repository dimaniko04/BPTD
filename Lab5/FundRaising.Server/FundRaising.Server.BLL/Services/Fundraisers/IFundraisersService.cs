using FundRaising.Server.BLL.DTOs.Fundraiser;
using FundRaising.Server.Core.Entities;

namespace FundRaising.Server.BLL.Services.Fundraisers;

public interface IFundraisersService
{
    Task<IEnumerable<FundraiserDto>> 
        GetAllUserFundraisers(Guid userId);
    Task<FundraiserDto> GetFundraiser(Guid id);
    Task<FundraiserDto> AddFundraiser(
        Guid userId,
        CreateFundraiserDto fundraiser);
    Task<FundraiserDto> UpdateFundraiser(
        Guid userId,
        Guid id,
        UpdateFundraiserDto fundraiser);
    Task DeleteFundraiser(Guid userId, Guid id);
    Task<FundraiserDto> Donate(
        Guid userId,
        Guid id,
        PaymentDto paymentDto);
}