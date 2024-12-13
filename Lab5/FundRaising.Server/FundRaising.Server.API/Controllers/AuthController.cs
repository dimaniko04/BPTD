using FundRaising.Server.BLL.DTOs.Authentication;
using FundRaising.Server.BLL.Services.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FundRaising.Server.API.Controllers;

[ApiController]
[Route("/auth")]
[AllowAnonymous]
public class AuthController: ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto registerDto)
    {
        var authResult = await _authService
            .RegisterAsync(registerDto);
        
        return Ok(authResult);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto loginDto)
    {
        var authResult = await _authService
            .LoginAsync(loginDto);
        
        return Ok(authResult);
    }
}