using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using RentACar.Data;
using RentACar.DTOs;
using RentACar.DTOs.Vehicle;
using RentACar.Entities;
using RentACar.Enums;

namespace RentACar.Services;

public class VehicleService : IVehicleService
{
    private readonly AppDbContext _context;
    private readonly IDistributedCache _cache;
    private readonly ILogger<VehicleService> _logger;

    public VehicleService(AppDbContext context, IDistributedCache cache, ILogger<VehicleService> logger)
    {
        _context = context;
        _cache = cache;
        _logger = logger;
    }

    public async Task<List<Vehicle>> GetVehicles(FilterVehiclesDto filterVehiclesDto)
    {
        string cacheKey = "vehicles:all";
        var q = _context.Vehicles.AsQueryable();
        
        if (filterVehiclesDto.IsEmpty())
        {
            var cachedData = await _cache.GetStringAsync(cacheKey);
            
            if (!string.IsNullOrEmpty(cachedData))
            {
                var cachedVehicles = JsonSerializer.Deserialize<List<Vehicle>>(cachedData);
                _logger.LogInformation("User fetched {vehiclesCount} vehicles from cache.", cachedVehicles.Count);
                return cachedVehicles;
            }
            
            return await q.ToListAsync();
        }
        
        // Filter query
        q = q
            .Where(v => filterVehiclesDto.Make == null || v.Make == filterVehiclesDto.Make)
            .Where(v => !filterVehiclesDto.FuelType.HasValue || v.FuelType == filterVehiclesDto.FuelType)
            .Where(v => !filterVehiclesDto.PriceStart.HasValue || v.PricePerDay >= filterVehiclesDto.PriceStart)
            .Where(v => !filterVehiclesDto.PriceEnd.HasValue || v.PricePerDay <= filterVehiclesDto.PriceEnd);
            
        if (filterVehiclesDto.StartDate.HasValue && filterVehiclesDto.EndDate.HasValue) {
            var rentedVehicleIds = await _context.Rentals
                .Where(r => r.StartDate < filterVehiclesDto.EndDate && r.EndDate > filterVehiclesDto.StartDate)
                .Select(r => r.VehicleId)
                .ToListAsync();

            q = q.Where(v => !rentedVehicleIds.Contains(v.Id));
        }
        
        
        var vehicles = await q.ToListAsync();
        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(vehicles), new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(30)));
        _logger.LogInformation("User fetched {vehiclesCount} vehicles.", vehicles.Count);
        
        return vehicles;
    }

    public async Task<Vehicle?> GetVehicleById(int vehicleId)
    {
        var vehicle = await _context.Vehicles.FirstOrDefaultAsync(v => v.Id == vehicleId);
        if (vehicle is null)
            _logger.LogWarning("User tried to find non-existent vehicle with provided ID {id}.", vehicleId);

        return vehicle;
    }
    
    public async Task<Result<Vehicle>> CreateVehicle(CreateVehicleDto createVehicleDto)
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
        int result = await _context.SaveChangesAsync();

        if (result > 0)
        {
            _logger.LogInformation("User created new vehicle with ID {id}.", vehicle.Id);
            
            await _cache.RemoveAsync("vechiles:all");
        
            return Result<Vehicle>.Success(vehicle);
        }

        _logger.LogWarning("User failed to create new vehicle!");
        
        return Result<Vehicle>.Failure(new Error("Vehicle cration failed!", "VehicleCreationFailed"));
    }

    public async Task<Result<Vehicle>> UpdateVehicle(int vehicleId, CreateVehicleDto createVehicleDto)
    {
        var vehicle = await GetVehicleById(vehicleId);
        if (vehicle is null)
        {
            return Result<Vehicle>.Failure(new Error("Vehicle not found.", "VehicleNotFound"));
        }
 
        vehicle.Make = createVehicleDto.Make;
        vehicle.Model = createVehicleDto.Model;
        vehicle.Year = createVehicleDto.Year;
        vehicle.FuelType = createVehicleDto.FuelType;
        vehicle.Color = createVehicleDto.Color;
        vehicle.PricePerDay = createVehicleDto.PricePerDay;
        vehicle.UpdatedAt = DateTime.UtcNow;
 
        _context.Vehicles.Update(vehicle);
        int result = await _context.SaveChangesAsync();

        if (result > 0)
        {
            _logger.LogInformation("User updated vehicle with ID {id}.", vehicle.Id);
        
            return Result<Vehicle>.Success(vehicle);
        }
 
        _logger.LogWarning("User failed to update vehicle!");
        
        return Result<Vehicle>.Failure(new Error("Vehicle update failed!", "VehicleUpdateFailed"));
    }
    
    public async Task<Result<Vehicle>> DeleteVehicle(int vehicleId)
    {
        var vehicle = await GetVehicleById(vehicleId);
        if (vehicle is null)
        {
            return Result<Vehicle>.Failure(new Error("Vehicle not found.", "VehicleNotFound"));
        }

        _context.Vehicles.Remove(vehicle);

        try
        {
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("User deleted vehicle with ID {id}.", vehicle.Id);

            return Result<Vehicle>.Success(vehicle);
        }
        catch (DbUpdateException _)
        {
            _logger.LogWarning("User failed to delete vehicle!");
            
            return Result<Vehicle>.Failure(new Error("Failed to delete the vehicle.", "VehicleDeleteFailed"));
        }
    }
}