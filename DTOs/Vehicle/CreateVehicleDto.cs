using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using RentACar.Enums;

namespace RentACar.DTOs.Vehicle;

public class CreateVehicleDto
{
    [Required(ErrorMessage = "Make is required.")]
    public string Make { get; set; }
    
    [Required(ErrorMessage = "Model is required.")]
    public string Model { get; set; }
    
    [Required(ErrorMessage = "Year is required.")]
    public int Year { get; set; }
    
    [Required(ErrorMessage = "Fuel type is required.")]
    public FuelType FuelType { get; set; }
    
    [Required(ErrorMessage = "Price per day is required.")]
    public double PricePerDay { get; set; }
    
    [Required(ErrorMessage = "Color type is required.")]
    public string Color { get; set; }
}