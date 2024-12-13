using FundRaising.Server.BLL.DTOs.User;

namespace FundRaising.Server.BLL.DTOs.Authentication;

public record AuthResponseDto(UserDto User, string Token);