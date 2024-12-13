using FundRaising.Server.BLL.DTOs.Authentication;

namespace FundRaising.Server.BLL.Services.Auth;

public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
    Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
}