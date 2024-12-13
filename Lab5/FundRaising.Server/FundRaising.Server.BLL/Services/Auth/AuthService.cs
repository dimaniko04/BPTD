using FundRaising.Server.BLL.DTOs.Authentication;
using FundRaising.Server.BLL.DTOs.User;
using FundRaising.Server.BLL.Exceptions;
using FundRaising.Server.BLL.Exceptions.Auth;
using FundRaising.Server.BLL.Interfaces.Repository;
using FundRaising.Server.BLL.Interfaces.Services;
using FundRaising.Server.Core.Entities;
using MapsterMapper;

namespace FundRaising.Server.BLL.Services.Auth;

public class AuthService: IAuthService
{
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IMapper _mapper;
    
    public AuthService(
        IJwtTokenGenerator jwtTokenGenerator,
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IMapper mapper)
    {
        _jwtTokenGenerator = jwtTokenGenerator;
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _mapper = mapper;
    }
    
    public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
    {
        var duplicate = await _userRepository
            .GetUserByEmailAsync(registerDto.Email);
        
        if (duplicate is not null)
        {
            throw new DuplicateEmailException();
        }
        
        var user = new User()
        {
            Id = Guid.NewGuid(),
            Email = registerDto.Email,
            PasswordHash = _passwordHasher.Hash(registerDto.Password)
        };
        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();
        
        var token = _jwtTokenGenerator.GenerateToken(user);
        var userDto = _mapper.Map<UserDto>(user);
        
        return new AuthResponseDto(userDto, token);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
    {
        var user = await _userRepository
            .GetUserByEmailAsync(loginDto.Email);

        if (user is null)
        {
            throw new AuthFailedException();
        }
        if (!_passwordHasher.VerifyHash(loginDto.Password, user.PasswordHash))
        {
            throw new AuthFailedException();
        }
        
        var token = _jwtTokenGenerator.GenerateToken(user);
        var userDto = _mapper.Map<UserDto>(user);
        
        return new AuthResponseDto(userDto, token);
    }
}