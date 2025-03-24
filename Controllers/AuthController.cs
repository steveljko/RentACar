using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using RentACar.DTOs.Auth;
using RentACar.Services;

namespace RentACar.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IUserService _userService;

    public AuthController(IAuthService authService, IUserService userService)
    {
        _authService = authService;
        _userService = userService;
    }
    
    [HttpPost("register")]
    [EnableRateLimiting("fixed")]
    public async Task<IActionResult> Register([FromBody] CreateUserDto createUserDto)
    {
        var result = await _authService.CreateUser(createUserDto);

        return result.Map<IActionResult>(
            onFailure: error => BadRequest(error),
            onSuccess: _ => Created("", new {
                Message = "Successfuly created new account."
            })
        );
    }
    
    [HttpPost("login")]
    [EnableRateLimiting("fixed")]
    public async Task<IActionResult> Login([FromBody] UserLoginDto userLoginDto)
    {
        var user = await _authService.Login(userLoginDto);
        if (user is null)
        {
            return BadRequest(new { Message = "Invalid username or password." });
        }
            
        return Ok(user);
    }
    
    [HttpGet("me")]
    [Authorize]
    [EnableRateLimiting("fixed")]
    public async Task<IActionResult> Me()
    {
        var username = User.Identity?.Name;
        if (string.IsNullOrEmpty(username))
        {
            return Unauthorized();
        }
        
        var user = await _userService.GetUserByUsername(username);
        if (user is null)
        {
            return NotFound();
        }

        var userDto = new UserDto
        {
            Name = user.Name,
            Username = user.Username,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
        };

        return Ok(userDto);
    }
}