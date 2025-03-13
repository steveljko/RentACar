using Microsoft.AspNetCore.Mvc;
using RentACar.DTOs.Auth;
using RentACar.Services;

namespace RentACar.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] CreateUserDto createUserDto)
    {
        var user = await _authService.CreateUser(createUserDto);
        if (user == null)
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
}