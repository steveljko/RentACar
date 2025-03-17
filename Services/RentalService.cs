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
    private readonly ICouponService _couponService;

    public RentalService(AppDbContext context, IUserService userService, IVehicleService vehicleService, ICouponService couponService)
    {
        _context = context;
        _userService = userService;
        _vehicleService = vehicleService;
        _couponService = couponService;
    }
    
    public async Task<Result<Rental>> CreateRental(int vehicleId, int userId, DateTime startDate, DateTime endDate, string? couponCode = null)
    {
        // Check if vehicle with this id exists and get it.
        var vehicle = await _vehicleService.GetVehicleById(vehicleId);
        if (vehicle is null)
        {
            return Result<Rental>.Failure(new Error("Vehicle not found.", "VehicleNotFound"));
        }
        
        // Confirm that the vehicle is available for the specified rental period.
        if (!await IsVehicleAvailable(vehicle, startDate, endDate))
        {
            return Result<Rental>.Failure(new Error("Vehicle is not available.", "VehicleNotAvailableForRent"));
        }
        
        var rentalDays = (endDate - startDate).Days;
        var totalPrice = rentalDays * vehicle.PricePerDay;
        int couponId = 0;
        
        if (!string.IsNullOrEmpty(couponCode))
        {
            var coupon = await _couponService.GetCouponByCode(couponCode);
            if (coupon is null)
            {
                return Result<Rental>.Failure(new Error("Invalid coupon code.", "InvalidCouponCode"));
            }
            
            // Verify if the user has already redeemed the coupon
            if (await _couponService.CheckIfCouponIsAlreadyReedemedByUser(couponCode, userId))
            {
                return Result<Rental>.Failure(new Error("You have already redeemed this coupon.", "CouponAlreadyReedemed"));
            }

            // Ensure the coupon is active
            if (coupon.Active is false)
            {
                return Result<Rental>.Failure(new Error("Coupon is currently deactivated.", "CouponInactive"));
            }

            couponId = coupon.Id;
            totalPrice -= totalPrice * (coupon.Discount / 100);
        }
        
        var rental = new Rental
        {
            VehicleId = vehicleId,
            RentedBy = userId,
            StartDate = startDate,
            EndDate = endDate,
            TotalPrice = totalPrice
        };
    
        await _context.Rentals.AddAsync(rental);
        await _context.SaveChangesAsync();

        // If the coupon is valid, add the user to the redemption record
        if (couponId != 0)
        {
            var couponRedeption = new CouponRedemption
            {
                RentalId = rental.Id,
                CouponId = couponId,
                UserId = userId,
            };
            
            await _context.CouponRedemptions.AddAsync(couponRedeption);
            await _context.SaveChangesAsync();
        }
        
        return Result<Rental>.Success(rental);
    }

    public async Task<Result<Rental>> CancelRent(int rentalId, int userId)
    {
        var rental = await _context.Rentals.FirstOrDefaultAsync(r => r.Id == rentalId && r.RentedBy == userId);
        if (rental is null)
        {
            return Result<Rental>.Failure(new Error("Rental not found or does not belong to the user.", "RentalDoesntBelongToUser"));
        }

        _context.Rentals.Remove(rental);
        await _context.SaveChangesAsync();

        return Result<Rental>.Success(rental);
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