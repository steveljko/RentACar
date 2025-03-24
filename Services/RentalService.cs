using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using RentACar.Data;
using RentACar.DTOs;
using RentACar.Entities;

namespace RentACar.Services;

public class RentalService : IRentalService
{
    private readonly AppDbContext _context;
    private readonly IDistributedCache _cache;
    private readonly ILogger<RentalService> _logger;
    private readonly IVehicleService _vehicleService;
    private readonly ICouponService _couponService;

    public RentalService(AppDbContext context, IDistributedCache cache, ILogger<RentalService> logger, IVehicleService vehicleService, ICouponService couponService)
    {
        _context = context;
        _cache = cache;
        _logger = logger;
        _vehicleService = vehicleService;
        _couponService = couponService;
    }

    public async Task<List<Rental>> ListAllRentalsForToday()
    {
        string cacheKey = "rentals:today";
        var cachedData = await _cache.GetStringAsync(cacheKey);

        if (!string.IsNullOrEmpty(cachedData))
        {
            var cachedRentals = JsonSerializer.Deserialize<List<Rental>>(cachedData);
            
            _logger.LogInformation("Found {cachedRentals} rentals for today from cache.", cachedRentals.Count);
            
            return cachedRentals;
        }

        var today = DateTime.UtcNow.Date;
        var tomorrow = today.AddDays(1);
        
        var rentals = await _context.Rentals
            .Include(r => r.Vehicle)
            .Include(r => r.Renter)
            .Where(r => r.StartDate >= today && r.StartDate < tomorrow)
            .ToListAsync();
        
        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(rentals), new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(30)));
        
        _logger.LogInformation("Found {count} rentals for today from DB.", rentals.Count);

        return rentals;
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
        
        
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var rentalDays = (endDate - startDate).Days;
            double totalPrice = rentalDays * vehicle.PricePerDay;
            Coupon? coupon = null;

            if (!string.IsNullOrEmpty(couponCode))
            {
                coupon = await _couponService.GetCouponByCode(couponCode);
                if (coupon is null)
                {
                    _logger.LogWarning("User tried to redeem invalid coupon with code {couponCode}.", couponCode);
                    return Result<Rental>.Failure(new Error("Invalid coupon code.", "InvalidCouponCode"));
                }

                // Verify if the user has already redeemed the coupon
                if (await _couponService.CheckIfCouponIsAlreadyReedemedByUser(couponCode, userId))
                {
                    _logger.LogWarning("User tried to redeem already redeemed coupon with code {couponCode}.",
                        couponCode);
                    return Result<Rental>.Failure(new Error("You have already redeemed this coupon.",
                        "CouponAlreadyReedemed"));
                }

                // Ensure the coupon is active
                if (coupon.Active is false)
                {
                    _logger.LogWarning("User tried to redeem deactivated coupon with code {couponCode}.", couponCode);
                    return Result<Rental>.Failure(new Error("Coupon is currently deactivated.", "CouponInactive"));
                }

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
            if (coupon is not null)
            {
                await CreateRedamptionForUser(rental, coupon, userId);
            }
            
            await transaction.CommitAsync();
            
            await _cache.RemoveAsync("rentals:today");

            _logger.LogInformation(
                "User successfully made rental for vehicle ID {vehicleId} from {startDate} to {endDate}.", vehicleId,
                startDate, endDate);
            
            return Result<Rental>.Success(rental);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw ex;
        }
    }

    public async Task<Result<Rental>> CancelRent(int rentalId, int userId)
    {
        var rental = await _context.Rentals.FirstOrDefaultAsync(r => r.Id == rentalId && r.RentedBy == userId);
        if (rental is null)
        {
            _logger.LogWarning("User tried to cancel rental with ID {id} that is not owned by him.", rentalId);
            return Result<Rental>.Failure(new Error("Rental not found or does not belong to the user.", "RentalDoesntBelongToUser"));
        }

        _context.Rentals.Remove(rental);
        await _context.SaveChangesAsync();

        _logger.LogInformation("User successfully canceled rental with ID {id}.", rentalId);
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

    private async Task<bool> CreateRedamptionForUser(Rental rental, Coupon coupon, int userId)
    {
        var couponRedeption = new CouponRedemption
        {
            RentalId = rental.Id,
            CouponId = coupon.Id,
            UserId = userId,
        };

        await _context.CouponRedemptions.AddAsync(couponRedeption);
        var result = await _context.SaveChangesAsync();

        if (result > 0)
        {
            _logger.LogInformation(
                "Successfully applied coupon '{couponCode}' (ID: {couponId}) for rental ID {rentalId} by user ID {userId}.",
                coupon.Code, coupon.Id, rental.Id, userId);
            return true;
        }

        return false;
    }
}