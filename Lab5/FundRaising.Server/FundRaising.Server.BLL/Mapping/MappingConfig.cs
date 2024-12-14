using FundRaising.Server.BLL.DTOs.Fundraiser;
using FundRaising.Server.BLL.DTOs.User;
using FundRaising.Server.Core.Entities;
using Mapster;

namespace FundRaising.Server.BLL.Mapping;

public class MappingConfig: IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<User, UserDto>();
        
        config.NewConfig<Fundraiser, FundraiserDto>()
            .Map(dest => dest.AmountRaised, src => src.AmountRaised / 100.0)
            .Map(dest => dest.Goal, src => src.Goal / 100.0);
        
        config.NewConfig<(Guid userId, CreateFundraiserDto dto), Fundraiser>()
            .Map(dest => dest.UserId, src => src.userId)
            .Map(dest => dest, src => src.dto)
            .Map(dest => dest.Id, _ => Guid.NewGuid())
            .Map(dest => dest.Goal, 
                src => (long)Math.Truncate(
                    double.Parse(src.dto.Goal) * 100.0));
        
        config.NewConfig<UpdateFundraiserDto, Fundraiser>()
            .Map(dest => dest.Goal, 
                src => (long)Math.Truncate(
                    double.Parse(src.Goal) * 100.0));
    }
}