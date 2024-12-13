using FundRaising.Server.Core.Entities;

namespace FundRaising.Server.BLL.Interfaces.Services;

public interface IJwtTokenGenerator
{
    string GenerateToken(User userId);
}