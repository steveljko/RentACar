using RentACar.DTOs;
using RentACar.DTOs.Auth;
using RentACar.Entities;

namespace RentACar.Services;

public interface IAuthService
{
    Task<Result<User>> CreateUser(CreateUserDto createUserDto);

    Task<TokenResponseDto?> Login(UserLoginDto userLoginDto);
}