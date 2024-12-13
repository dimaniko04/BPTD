using FundRaising.Server.BLL.DTOs.Fundraiser;
using FundRaising.Server.BLL.Exceptions.Fundraiser;
using FundRaising.Server.BLL.Interfaces.Repository;
using FundRaising.Server.Core.Entities;
using MapsterMapper;

namespace FundRaising.Server.BLL.Services.Fundraisers;

public class FundraisersService: IFundraisersService
{
    private readonly IFundraisersRepository _fundraisersRepository;
    private readonly IMapper _mapper;
    
    public FundraisersService(
        IFundraisersRepository fundraisersRepository,
        IMapper mapper)
    {
        _fundraisersRepository = fundraisersRepository;
        _mapper = mapper;
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
        var fundraiser = _mapper
            .Map<Fundraiser>((userId, createFundraiserDto));
        
        await _fundraisersRepository.AddAsync(fundraiser);
        await _fundraisersRepository.SaveChangesAsync();
    }

    public async Task UpdateFundraiser(
        Guid userId,
        Guid fundraiserId,
        CreateFundraiserDto createFundraiserDto)
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
}