using RentACar.Entities;

namespace RentACar.DTOs;

public class RentalResult
{
    public Rental? Rental { get; set; }
    public bool Success { get; set; }
    public string? Error { get; set; }
}