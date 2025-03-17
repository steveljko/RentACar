using RentACar.DTOs.Vehicle;
using RentACar.Entities;
using RentACar.Enums;

namespace RentACar.Services;

public interface IVehicleService
{
    Task<List<Vehicle>> GetVehicles(string? make, FuelType? fuelType, int? priceStart, int? priceEnd, DateTime? startDate, DateTime? endDate);
    Task<Vehicle?> GetVehicleById(int vehicleId);
    Task<Vehicle?> CreateVehicle(CreateVehicleDto createVehicleDto);
    Task<Vehicle?> UpdateVehicle(int vehicleId, CreateVehicleDto createVehicleDto);
    Task<bool?> DeleteVehicle(int vehicleId);
}