using RentACar.DTOs;
using RentACar.Entities;

namespace RentACar.Services;

public interface IRentalService
{
    Task<Result<Rental>> CreateRental(int vehicleId, int userId, DateTime startDate, DateTime endDate, string? couponCode = null);
    Task<Result<Rental>> CancelRent(int rentalId, int userId);
}
