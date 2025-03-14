using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RentACar.Config;
using RentACar.Data;
using RentACar.DTOs.Auth;
using RentACar.Entities;
using RentACar.Enums;
using Claim = System.Security.Claims.Claim;

namespace RentACar.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IUserService _userService;
    private readonly JwtConfiguration _jwtConfig;

    public AuthService(AppDbContext context, IUserService userService, IOptions<JwtConfiguration> jwtConfig)
    {
        _context = context;
        _userService = userService;
        _jwtConfig = jwtConfig.Value;
    }

    public async Task<User?> CreateUser(CreateUserDto createUserDto)
    {
        var existingUser = await _userService.GetUserByUsername(createUserDto.Username);
        if (existingUser != null)
        {
            return null;
        }
        
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password);
        
        var newUser = new User
        {
            Name = createUserDto.Name,
            Username = createUserDto.Username,
            Password = hashedPassword,
            Role = UserRole.User,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        _context.Users.AddAsync(newUser);
        await _context.SaveChangesAsync();

        return newUser;
    }

    public async Task<TokenResponseDto?> Login(UserLoginDto userLoginDto)
    {
        var user = await _userService.GetUserByUsername(userLoginDto.Username);
        if (user is null)
        {
            return null;
        }
        
        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(userLoginDto.Password, user.Password);
        if (!isPasswordValid)
        {
            return null;
        }

        var token = GenerateJwtToken(user);

        return new TokenResponseDto
        {
            AccessToken = token,
        };
    }
    
    private string GenerateJwtToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtConfig.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        
        var token = new JwtSecurityToken(
            issuer: _jwtConfig.Issuer,
            audience: _jwtConfig.Audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(_jwtConfig.ExpireMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}