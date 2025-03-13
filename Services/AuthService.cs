using RentACar.Data;
using RentACar.DTOs.Auth;
using RentACar.Entities;
using RentACar.Enums;

namespace RentACar.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IUserService _userService;

    public AuthService(AppDbContext context, IUserService userService)
    {
        _context = context;
        _userService = userService;
    }

    public async Task<User?> CreateUser(CreateUserDto createUserDto)
    {
        var existingUser = await _userService.GetUserByUsername(createUserDto.Username);
        
        if (existingUser != null)
        {
            return null;
        }
        
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password);
        
        var newUser = new User()
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
}