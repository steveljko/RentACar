using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentACar.DTOs.Coupon;
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
        
        var coupon = await _couponService.CreateCoupon(createCouponDto, user);
        if (coupon is null)
        {
            return BadRequest();
        }

        return Ok(coupon);
    }
    
    [HttpPatch("{couponId}/toggle")]
    [Authorize]
    public async Task<IActionResult> ToggleCoupon(int couponId)
    {
        var coupon = await _couponService.ToggleCouponActiveState(couponId);
        
        return Ok(coupon);
    }
}