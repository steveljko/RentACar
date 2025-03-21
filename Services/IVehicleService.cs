using Microsoft.AspNetCore.Mvc;
using RentACar.DTOs;
using RentACar.DTOs.Vehicle;
using RentACar.Entities;
using RentACar.Enums;

namespace RentACar.Services;

public interface IVehicleService
{
    Task<List<Vehicle>> GetVehicles(FilterVehiclesDto filterVehiclesDto);
    Task<Vehicle?> GetVehicleById(int vehicleId);
    Task<Result<Vehicle>> CreateVehicle(CreateVehicleDto createVehicleDto);
    Task<Result<Vehicle>> UpdateVehicle(int vehicleId, CreateVehicleDto createVehicleDto);
    Task<Result<Vehicle>> DeleteVehicle(int vehicleId);
}