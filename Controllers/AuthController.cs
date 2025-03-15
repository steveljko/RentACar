using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    public async Task<IActionResult> Register([FromBody] CreateUserDto createUserDto)
    {
        var user = await _authService.CreateUser(createUserDto);
        if (user is null)
        {
            return BadRequest(new { Message = "User already exists. " });
        }

        var response = new UserDto()
        {
            Name = user.Name,
            Username = user.Username,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
        };
        
        return Created(string.Empty, response);
    }
    
    [HttpPost("login")]
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