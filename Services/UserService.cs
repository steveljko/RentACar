using Microsoft.EntityFrameworkCore;
using RentACar.Data;
using RentACar.Entities;

namespace RentACar.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserService(AppDbContext context, IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<User?> GetUserByUsername(string username)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<User?> FetchCurrentUser()
    {       
        var username = _httpContextAccessor.HttpContext?.User.Identity?.Name;
        if (username is null)
        {
            return null;
        }
        
        var user = await GetUserByUsername(username);
        return user;
    }
}