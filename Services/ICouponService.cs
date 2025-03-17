using RentACar.DTOs;
using RentACar.DTOs.Coupon;
using RentACar.Entities;

namespace RentACar.Services;

public interface ICouponService
{
    Task<Coupon?> GetCouponById(int couponId);
    Task<Coupon?> GetCouponByCode(string code);
    Task<Result<Coupon>> CreateCoupon(CreateCouponDto createCouponDto, User user);
    Task<Result<Coupon>> ToggleCouponActiveState(int couponId);
    Task<bool> CheckIfCouponIsAlreadyReedemedByUser(string couponCode, int userId);
}