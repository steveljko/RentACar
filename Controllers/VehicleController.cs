using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentACar.DTOs.Vehicle;
using RentACar.Enums;
using RentACar.Services;

namespace RentACar.Controllers;

[ApiController]
[Route("api/v1/vehicles")]
public class VehicleController : ControllerBase
{
    private readonly IVehicleService _vehicleService;

    public VehicleController(IVehicleService vehicleService)
    {
        _vehicleService = vehicleService;
    }

    [HttpGet]
    public async Task<IActionResult> GetVehicles(string? make, FuelType? fuelType, int? priceStart,
        int? priceEnd, DateTime? startDate, DateTime? endDate)
    {
        var vehicles = await _vehicleService.GetVehicles(make, fuelType, priceStart, priceEnd, startDate, endDate);

        return Ok(vehicles);
    }
    
    [HttpPost]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> CreateVehicle([FromBody] CreateVehicleDto createVehicleDto)
    {
         var vehicle = await _vehicleService.CreateVehicle(createVehicleDto);

         return Ok(vehicle);
    }
    
    [HttpPut("{vehicleId}")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> UpdateVehicle(int vehicleId, [FromBody] CreateVehicleDto createVehicleDto)
    {
         var vehicle = await _vehicleService.UpdateVehicle(vehicleId, createVehicleDto);
         if (vehicle is null)
         {
             return BadRequest();
         }

         var vehicleDto = new VehicleDto
         {
             Make = vehicle.Make,
             Model = vehicle.Model,
             Year = vehicle.Year,
             CreatedAt = vehicle.CreatedAt,
             UpdatedAt = vehicle.UpdatedAt,
         };

         return Ok(vehicle);
    }


    [HttpDelete("{vehicleId}")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> DeleteVehicle(int vehicleId)
    {
         var result = await _vehicleService.DeleteVehicle(vehicleId);
         if (result is null)
         {
             return NotFound(new { message = "Vehicle not found." });
         }

         if (result is false)
         {
             return BadRequest(new { message = "Failed to delete the vehicle." });
         }

         return Ok(new { message = "Vehicle has been successfully deleted." });
    }
}