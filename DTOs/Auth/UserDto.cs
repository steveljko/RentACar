using System.Text.Json.Serialization;
using RentACar.Enums;

namespace RentACar.DTOs.Auth;

public class UserDto
{
    public string Name { get; set; }
    public string Username { get; set; }
    [JsonIgnore]
    public UserRole Role { get; set; }
    public string UserRole => Role.ToString().ToLower();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}