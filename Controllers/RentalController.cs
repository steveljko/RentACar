using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentACar.DTOs;
using RentACar.DTOs.Auth;
using RentACar.DTOs.Vehicle;
using RentACar.Services;

namespace RentACar.Controllers;

[ApiController]
[Route("api/v1/rentals")]
public class RentalController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IRentalService _rentalService;

    public RentalController(IUserService userService, IRentalService rentalService)
    {
        _userService = userService;
        _rentalService = rentalService;
    }

    [HttpPost("{vehicleId}")]
    [Authorize]
    public async Task<IActionResult> Rent(int vehicleId, [FromBody] CreateRentalDto createRentalDto)
    {
        var user = await _userService.FetchCurrentUser();
        if (user is null)
        {
            return Unauthorized();
        }
        
        var result = await _rentalService.CreateRental(vehicleId, user.Id, createRentalDto.StartDate, createRentalDto.EndDate);
        if (!result.Success)
        {
            return BadRequest(new { Message = result.Error });
        }

        var rental = new RentalDto
        {
            StartDate = result.Rental.StartDate,
            EndDate = result.Rental.EndDate,
            Vehicle = new VehicleDto
            {
                Make = result.Rental.Vehicle.Make,
                Model = result.Rental.Vehicle.Model,
                Year = result.Rental.Vehicle.Year,
            },
            Renter = new UserDto()
            {
                Name = result.Rental.Renter.Name,
                Username = result.Rental.Renter.Username,
                CreatedAt = result.Rental.Renter.CreatedAt,
                UpdatedAt = result.Rental.Renter.UpdatedAt,
            }
        };

        return Ok(new
        {
            Message = "Vehicle rented successfully.",
            Rental = rental,
        });
    }
}