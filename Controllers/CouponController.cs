using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentACar.DTOs.Coupon;
using RentACar.Entities;
using RentACar.Services;

namespace RentACar.Controllers;

[ApiController]
[Route("api/v1/coupons")]
public class CouponController : ControllerBase
{
    private readonly ICouponService _couponService;
    private readonly IUserService _userService;

    public CouponController(ICouponService couponService, IUserService userService)
    {
        _couponService = couponService;
        _userService = userService;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateCoupon(CreateCouponDto createCouponDto)
    {
        var user = await _userService.FetchCurrentUser();
        if (user is null)
            return Unauthorized();
        
        var result = await _couponService.CreateCoupon(createCouponDto, user);
        
        return result.Map<IActionResult>(
            onFailure: error => BadRequest(error),
            onSuccess: coupon => Ok(new {
                Message = "Coupon created successfully.",
                Coupon = coupon
            })
        );
    }
    
    [HttpPatch("{couponId}/toggle")]
    [Authorize]
    public async Task<IActionResult> ToggleCoupon(int couponId)
    {
        var result = await _couponService.ToggleCouponActiveState(couponId);

        return result.Map<IActionResult>(
            onFailure: error => BadRequest(error),
            onSuccess: coupon => Ok(new {
                Message = $"Coupon state is currently {(coupon.Active ? "active" : "inactive")}.",
                Coupon = coupon,
            })
        );
    }
}