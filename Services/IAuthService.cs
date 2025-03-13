using RentACar.DTOs.Auth;
using RentACar.Entities;

namespace RentACar.Services;

public interface IAuthService
{
    Task<User?> CreateUser(CreateUserDto createUserDto);
}