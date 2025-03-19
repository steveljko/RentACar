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
         var result = await _vehicleService.CreateVehicle(createVehicleDto);

         return result.Map<IActionResult>(
             onFailure: error => BadRequest(error),
             onSuccess: vehicle =>
             {
                 var vehicleDto = new VehicleDto
                 {
                     Make = vehicle.Make,
                     Model = vehicle.Model,
                     Year = vehicle.Year,
                     CreatedAt = vehicle.CreatedAt,
                     UpdatedAt = vehicle.UpdatedAt,
                 };

                 return Created("", new
                 {
                     Message = "New vehicle has been successfully created.",
                     Vehicle = vehicleDto
                 });
             }
         );
    }
    
    [HttpPut("{vehicleId}")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> UpdateVehicle(int vehicleId, [FromBody] CreateVehicleDto createVehicleDto)
    {
         var result = await _vehicleService.UpdateVehicle(vehicleId, createVehicleDto);

         return result.Map<IActionResult>(
             onFailure: error => BadRequest(error),
             onSuccess: vehicle =>
             {
                 var vehicleDto = new VehicleDto
                 {
                     Make = vehicle.Make,
                     Model = vehicle.Model,
                     Year = vehicle.Year,
                     CreatedAt = vehicle.CreatedAt,
                     UpdatedAt = vehicle.UpdatedAt,
                 };
                 
                 return Ok(new
                 {
                     Message = "Vehicle has been successfully deleted.",
                     Vehicle = vehicleDto
                 });
             });
    }


    [HttpDelete("{vehicleId}")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> DeleteVehicle(int vehicleId)
    {
         var result = await _vehicleService.DeleteVehicle(vehicleId);

         return result.Map<IActionResult>(
             onFailure: error => BadRequest(error),
             onSuccess: _ => Ok(new {
                 Message = "Vehicle has been successfully deleted."
             })
         );
    }
}