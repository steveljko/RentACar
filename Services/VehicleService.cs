using Microsoft.EntityFrameworkCore;
using RentACar.Data;
using RentACar.DTOs.Vehicle;
using RentACar.Entities;
using RentACar.Enums;

namespace RentACar.Services;

public class VehicleService : IVehicleService
{
    private readonly AppDbContext _context;
    private readonly ILogger<VehicleService> _logger;

    public VehicleService(AppDbContext context, ILogger<VehicleService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<Vehicle>> GetVehicles(
        string? make, FuelType? fuelType,
        int? priceStart, int? priceEnd,
        DateTime? startDate, DateTime? endDate)
    {
        var q = _context.Vehicles.AsQueryable();
        
        if (make != null)
        {
            q = q.Where(v => v.Make == make);
        }
        
        if (fuelType.HasValue)
        {
            q = q.Where(v => v.FuelType == fuelType);
        }
        
        if (priceStart.HasValue)
        {
            q = q.Where(v => v.PricePerDay >= priceStart);
        }
        
        if (priceEnd.HasValue)
        {
            q = q.Where(v => v.PricePerDay <= priceEnd);
        }
        
        if (startDate.HasValue && endDate.HasValue) {
            var rentedVehicleIds = await _context.Rentals
                .Where(r => r.StartDate < endDate && r.EndDate > startDate)
                .Select(r => r.VehicleId)
                .ToListAsync();

            q = q.Where(v => !rentedVehicleIds.Contains(v.Id));
        }
        
        var vehicles = await q.ToListAsync();
        
        _logger.LogInformation("User fetched {vehiclesCount} vehicles.", vehicles.Count);
        
        return vehicles;
    }

    public async Task<Vehicle?> GetVehicleById(int vehicleId)
    {
        var vehicle = await _context.Vehicles.FirstOrDefaultAsync(v => v.Id == vehicleId);
        if (vehicle is null)
            _logger.LogInformation("User tried to find non-existent vehicle with provided ID {id}.", vehicleId);

        return vehicle;
    }
    
    public async Task<Vehicle?> CreateVehicle(CreateVehicleDto createVehicleDto)
    {
        var vehicle = new Vehicle
        {
            Make = createVehicleDto.Make,
            Model = createVehicleDto.Model,
            Year = createVehicleDto.Year,
            FuelType = createVehicleDto.FuelType,
            Color = createVehicleDto.Color,
            PricePerDay = createVehicleDto.PricePerDay,
            IsAvailable = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };
        
        await _context.Vehicles.AddAsync(vehicle);
        await _context.SaveChangesAsync();

        _logger.LogInformation("User created new vehicle with ID {id}.", vehicle.Id);
        
        return vehicle;
    }

    public async Task<Vehicle?> UpdateVehicle(int vehicleId, CreateVehicleDto createVehicleDto)
    {
        var vehicle = await GetVehicleById(vehicleId);
        if (vehicle is null)
        {
            return null;
        }
 
        vehicle.Make = createVehicleDto.Make;
        vehicle.Model = createVehicleDto.Model;
        vehicle.Year = createVehicleDto.Year;
        vehicle.FuelType = createVehicleDto.FuelType;
        vehicle.Color = createVehicleDto.Color;
        vehicle.PricePerDay = createVehicleDto.PricePerDay;
        vehicle.UpdatedAt = DateTime.UtcNow;
 
        _context.Vehicles.Update(vehicle);
        await _context.SaveChangesAsync();
 
        _logger.LogInformation("User updated vehicle with ID {id}.", vehicle.Id);
        
        return vehicle;
    }
    
    public async Task<bool?> DeleteVehicle(int vehicleId)
    {
        var vehicle = await GetVehicleById(vehicleId);
        if (vehicle is null)
        {
            return null;
        }

        _context.Vehicles.Remove(vehicle);

        try
        {
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("User deleted vehicle with ID {id}.", vehicle.Id);
        }
        catch (DbUpdateException _)
        {
            return false;
        }

        return true;
    }
}