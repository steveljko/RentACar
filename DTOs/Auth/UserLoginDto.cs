using System.ComponentModel.DataAnnotations;

namespace RentACar.DTOs.Auth;

public class UserLoginDto
{
    [Required(ErrorMessage = "Username is required.")]
    [MinLength(8, ErrorMessage = "Username must be at least 8 characters long.")]
    [MaxLength(32, ErrorMessage = "Username cannot exceed 32 characters.")]
    public string Username { get; set; }
    
    [Required(ErrorMessage = "Password is required.")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
    [MaxLength(128, ErrorMessage = "Password cannot exceed 128 characters.")]
    public string Password { get; set; }
}