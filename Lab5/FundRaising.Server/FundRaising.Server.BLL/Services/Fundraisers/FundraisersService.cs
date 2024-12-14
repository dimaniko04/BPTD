using FundRaising.Server.BLL.DTOs.Fundraiser;
using FundRaising.Server.BLL.Exceptions.Fundraiser;
using FundRaising.Server.BLL.Interfaces.Repository;
using FundRaising.Server.BLL.Interfaces.Services;
using FundRaising.Server.Core.Entities;
using MapsterMapper;

namespace FundRaising.Server.BLL.Services.Fundraisers;

public class FundraisersService: IFundraisersService
{
    private readonly IFundraisersRepository _fundraisersRepository;
    private readonly ILiqPay _liqPay;
    private readonly IMapper _mapper;
    
    public FundraisersService(
        IFundraisersRepository fundraisersRepository,
        ILiqPay liqPay,
        IMapper mapper)
    {
        _fundraisersRepository = fundraisersRepository;
        _mapper = mapper;
        _liqPay = liqPay;
    }
    
    public async Task<IEnumerable<FundraiserDto>> 
        GetAllUserFundraisers(Guid userId)
    {
        var fundraisers = await _fundraisersRepository
            .GetAllUserFundraisers(userId);
        
        var fundraisersDto = _mapper
            .Map<IEnumerable<FundraiserDto>>(fundraisers);
        
        return fundraisersDto;
    }

    public async Task<FundraiserDto> GetFundraiser(Guid id)
    {
        var fundraiser = await _fundraisersRepository
            .GetByIdAsync(id);

        if (fundraiser == null)
        {
            throw new FundraiserNotFound(id);
        }
        
        var fundraiserDto = _mapper.Map<FundraiserDto>(fundraiser);
        
        return fundraiserDto;
    }

    public async Task AddFundraiser(
        Guid userId,
        CreateFundraiserDto createFundraiserDto)
    {
        if (!double.TryParse(createFundraiserDto.Goal, out _))
        {
            throw new InvalidAmount();
        }
        
        var fundraiser = _mapper
            .Map<Fundraiser>((userId, createFundraiserDto));
        
        await _fundraisersRepository.AddAsync(fundraiser);
        await _fundraisersRepository.SaveChangesAsync();
    }

    public async Task UpdateFundraiser(
        Guid userId,
        Guid fundraiserId,
        UpdateFundraiserDto createFundraiserDto)
    {
        var fundraiser = await _fundraisersRepository
            .GetByIdAsync(fundraiserId);

        if (fundraiser == null)
        {
            throw new FundraiserNotFound(fundraiserId);
        }
        if (fundraiser.UserId != userId)
        {
            throw new FundraiserAccessDenied();
        }
        if (!double.TryParse(createFundraiserDto.Goal, out _))
        {
            throw new InvalidAmount();
        }
        
        _mapper.Map(createFundraiserDto, fundraiser);
        _fundraisersRepository.Update(fundraiser);
        await _fundraisersRepository.SaveChangesAsync();
    }

    public async Task DeleteFundraiser(Guid userId, Guid id)
    {
        var fundraiser = await _fundraisersRepository
            .GetByIdAsync(id);

        if (fundraiser == null)
        {
            throw new FundraiserNotFound(id);
        }
        if (fundraiser.UserId != userId)
        {
            throw new FundraiserAccessDenied();
        }
        
        _fundraisersRepository.Delete(fundraiser);
        await _fundraisersRepository.SaveChangesAsync();
    }

    public async Task Donate(
        Guid userId, Guid id, PaymentDto paymentDto)
    {
        if (!double.TryParse(paymentDto.Amount, out var amount))
        {
            throw new InvalidAmount();
        }

        var fundraiser = await _fundraisersRepository
            .GetByIdAsync(id);

        if (fundraiser == null)
        {
            throw new FundraiserNotFound(id);
        }
        
        await _liqPay.Payment(
            paymentDto.Description,
            paymentDto.Amount,
            paymentDto.Card,
            paymentDto.CardExpiryDate,
            paymentDto.CardCvv,
            "UAH",
            Guid.NewGuid(),
            "pay"
        );
        
        var integerAmount = (long)Math.Truncate(amount * 100);
        
        var donation = new Donation
        {
            Amount = integerAmount,
            Description = paymentDto.Description,
            UserId = userId,
            FundraiserId = id
        };
        fundraiser.AmountRaised += integerAmount;
        
        _fundraisersRepository.Update(fundraiser);
        await _fundraisersRepository.AddDonation(donation);
        await _fundraisersRepository.SaveChangesAsync();
    }
}