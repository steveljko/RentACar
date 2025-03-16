namespace RentACar.DTOs;

public class CreateRentalDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? CouponCode { get; set; }
}