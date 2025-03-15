using Microsoft.EntityFrameworkCore;
using RentACar.Data;
using RentACar.DTOs;
using RentACar.Entities;

namespace RentACar.Services;

public class RentalService : IRentalService
{
    private readonly AppDbContext _context;
    private readonly IUserService _userService;
    private readonly IVehicleService _vehicleService;

    public RentalService(AppDbContext context, IUserService userService, IVehicleService vehicleService)
    {
        _context = context;
        _userService = userService;
        _vehicleService = vehicleService;
    }
    
    public async Task<RentalResult> CreateRental(int vehicleId, int userId, DateTime startDate, DateTime endDate)
    {
        // Validate that the start date is not set in the past.
        if (startDate < DateTime.Now)
        {
            return new RentalResult
            {
                Success = false,
                Error = "The start date cannot be in the past."
            };
        }

        // Ensure that the start date occurs before the end date.
        if (startDate > endDate)
        {
            return new RentalResult
            {
                Success = false,
                Error = "The start date must be earlier than the end date."
            };
        }
        
        // Check if vehicle with this id exists and get it.
        var vehicle = await _vehicleService.GetVehicleById(vehicleId);
        if (vehicle is null)
        {
            return new RentalResult
            {
                Success = false,
                Error = "Vehicle not found."
            };
        }
        
        // Confirm that the vehicle is available for the specified rental period.
        if (!await IsVehicleAvailable(vehicle, startDate, endDate))
        {
            return new RentalResult
            {
                Success = false,
                Error = "Vehicle is not available." 
            };
        }
        
        var rental = new Rental
        {
            VehicleId = vehicleId,
            RentedBy = userId,
            StartDate = startDate,
            EndDate = endDate
        };
    
        _context.Rentals.AddAsync(rental);
        await _context.SaveChangesAsync();

        return new RentalResult
        {
            Success = true,
            Rental = rental
        };
    }
    
    private async Task<bool> IsVehicleAvailable(Vehicle vehicle, DateTime startDate, DateTime endDate)
    {
        // Check if vehicle is available for renting.
        if (!vehicle.IsAvailable)
        {
            return false;
        }

        // Check if vehicle is rented for this period. 
        return !await _context.Rentals
            .AnyAsync(r => 
                r.VehicleId == vehicle.Id &&
                r.StartDate < endDate &&
                r.EndDate > startDate);
    }
}