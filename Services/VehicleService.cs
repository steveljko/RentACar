using Microsoft.EntityFrameworkCore;
using RentACar.Data;
using RentACar.DTOs.Vehicle;
using RentACar.Entities;

namespace RentACar.Services;

public class VehicleService : IVehicleService
{
    private readonly AppDbContext _context;

    public VehicleService(AppDbContext context)
    {
        _context = context;
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
            IsAvailable = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };
        
        _context.Vehicles.AddAsync(vehicle);
        await _context.SaveChangesAsync();

        return vehicle;
    }

    public async Task<Vehicle?> UpdateVehicle(int vehicleId, CreateVehicleDto createVehicleDto)
    {
        var vehicle = await _context.Vehicles.FindAsync(vehicleId);
        if (vehicle is null)
        {
            return null;
        }
 
        vehicle.Make = createVehicleDto.Make;
        vehicle.Model = createVehicleDto.Model;
        vehicle.Year = createVehicleDto.Year;
        vehicle.FuelType = createVehicleDto.FuelType;
        vehicle.Color = createVehicleDto.Color;
        vehicle.UpdatedAt = DateTime.UtcNow;
 
        _context.Vehicles.Update(vehicle);
        await _context.SaveChangesAsync();
 
        return vehicle;
    }
    
    public async Task<bool?> DeleteVehicle(int vehicleId)
    {
        var vehicle = await _context.Vehicles.FindAsync(vehicleId);
        if (vehicle is null)
        {
            return null;
        }

        _context.Vehicles.Remove(vehicle);
        
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException _)
        {
            return false;
        }

        return true;
    }
}