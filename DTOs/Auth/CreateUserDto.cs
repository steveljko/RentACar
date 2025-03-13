using System.ComponentModel.DataAnnotations;

namespace RentACar.DTOs.Auth;

public class CreateUserDto
{
    [Required(ErrorMessage = "Name is required.")]
    [MinLength(8, ErrorMessage = "Name must be at least 8 characters long.")]
    [MaxLength(32, ErrorMessage = "Name cannot exceed 32 characters.")]
    public string Name { get; set; }
    
    [Required(ErrorMessage = "Username is required.")]
    [MinLength(8, ErrorMessage = "Username must be at least 8 characters long.")]
    [MaxLength(32, ErrorMessage = "Username cannot exceed 32 characters.")]
    public string Username { get; set; }
    
    [Required(ErrorMessage = "Password is required.")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
    [MaxLength(128, ErrorMessage = "Password cannot exceed 128 characters.")]
    public string Password { get; set; }
    
    [Required(ErrorMessage = "Confirm password is required.")]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; }
}