using System.ComponentModel.DataAnnotations;
using RentACar.Attributes.Validation;

namespace RentACar.DTOs;

public class CreateRentalDto
{
    [Required(ErrorMessage = "Start date is required.")]
    [DataType(DataType.Date, ErrorMessage = "Start date must be valid date type.")]
    [DateMustBeInFuture]
    public DateTime StartDate { get; set; }
    
    [Required(ErrorMessage = "End date is required.")]
    [DataType(DataType.Date, ErrorMessage = "End date must be valid date type.")]
    [DateGreaterThan("StartDate", ErrorMessage = "End date must be greater than start date.")]
    public DateTime EndDate { get; set; }
    
    public string? CouponCode { get; set; }
}