using RentACar.DTOs.Auth;
using RentACar.DTOs.Vehicle;

namespace RentACar.DTOs;

public class RentalDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public VehicleDto Vehicle { get; set; }
    public UserDto Renter { get; set; }
}