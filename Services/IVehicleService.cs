using RentACar.DTOs.Vehicle;
using RentACar.Entities;

namespace RentACar.Services;

public interface IVehicleService
{
    Task<Vehicle?> CreateVehicle(CreateVehicleDto createVehicleDto);
    Task<Vehicle?> UpdateVehicle(int vehicleId, CreateVehicleDto createVehicleDto);
    Task<bool?> DeleteVehicle(int vehicleId);
}