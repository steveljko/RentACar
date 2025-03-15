using RentACar.Entities;

namespace RentACar.Services;

public interface IUserService
{
    Task<User?> GetUserByUsername(string username);
    Task<User?> FetchCurrentUser();
}