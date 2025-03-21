using System.ComponentModel.DataAnnotations;
using RentACar.Attributes.Validation;
using RentACar.Enums;

namespace RentACar.DTOs.Vehicle;

public class FilterVehiclesDto
{
    [StringLength(100, ErrorMessage = "Make cannot be longer than 100 characters.")]
    public string? Make { get; set; }

    public FuelType? FuelType { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Price start must be a non-negative number.")]
    public int? PriceStart { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Price end must be a non-negative number.")]
    public int? PriceEnd { get; set; }

    [DataType(DataType.Date)]
    public DateTime? StartDate { get; set; }

    [DataType(DataType.Date)]
    [DateGreaterThan("StartDate", ErrorMessage = "End date must be greater than start date.")]
    public DateTime? EndDate { get; set; }

    public bool IsEmpty()
    {
        return string.IsNullOrEmpty(Make) &&
               !FuelType.HasValue &&
               !PriceStart.HasValue &&
               !PriceEnd.HasValue &&
               !StartDate.HasValue &&
               !EndDate.HasValue;
    }
}