using RentACar.DTOs;
using RentACar.Entities;

namespace RentACar.Services;

public interface IRentalService
{
    Task<RentalResult> CreateRental(int vehicleId, int userId, DateTime startDate, DateTime endDate);
}
