namespace RentACar.DTOs.Coupon;

public class CreateCouponDto
{
    public string Code { get; set; }
    public string Description { get; set; }
    public float Discount { get; set; }
    public string? CouponCode { get; set; }
}