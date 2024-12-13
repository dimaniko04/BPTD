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
        
        config.NewConfig<Fundraiser, FundraiserDto>();
        config.NewConfig<(Guid id, CreateFundraiserDto dto), Fundraiser>()
            .Map(dest => dest.UserId, src => src.id)
            .Map(dest => dest, src => src.dto)
            .Map(dest => dest.Id, _ => Guid.NewGuid());
    }
}