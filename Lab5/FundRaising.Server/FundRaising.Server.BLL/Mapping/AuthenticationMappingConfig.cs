using FundRaising.Server.BLL.DTOs.User;
using FundRaising.Server.Core.Entities;
using Mapster;

namespace FundRaising.Server.BLL.Mapping;

public class AuthenticationMappingConfig: IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<User, UserDto>();
    }
}