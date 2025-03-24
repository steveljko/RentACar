using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using RentACar.DTOs;
using RentACar.DTOs.Auth;
using RentACar.DTOs.Vehicle;
using RentACar.Enums;
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

    [HttpGet]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<IActionResult> ListAllRentalsForToday()
    {
        var rentals = await _rentalService.ListAllRentalsForToday();

        return Ok(rentals);
    }

    [HttpPost("{vehicleId}")]
    [Authorize]
    [EnableRateLimiting("fixed")]
    public async Task<IActionResult> Rent(int vehicleId, [FromBody] CreateRentalDto createRentalDto)
    {
        var user = await _userService.FetchCurrentUser();
        if (user is null)
        {
            return Unauthorized();
        }
        
        var result = await _rentalService.CreateRental(vehicleId, user.Id, createRentalDto.StartDate, createRentalDto.EndDate, createRentalDto?.CouponCode);

        return result.Map<IActionResult>(
            onFailure: error => BadRequest(error),
            onSuccess: rental => Created("", new {
                Message = "Vehicle is succesfully rented.",
                Rental = rental
            })
        );
    }

    [HttpDelete("{rentalId}")]
    [Authorize]
    [EnableRateLimiting("fixed")]
    public async Task<IActionResult> CancelRent(int rentalId)
    {
        var user = await _userService.FetchCurrentUser();
        if (user is null)
        {
            return Unauthorized();
        }
        
        var result = await _rentalService.CancelRent(rentalId, user.Id);

        return result.Map<IActionResult>(
            onFailure: error => BadRequest(error),
            onSuccess: rental => Ok(new {
                Message = "Rental canceled successfully.",
                Rental = rental
            })
        );
    }
}