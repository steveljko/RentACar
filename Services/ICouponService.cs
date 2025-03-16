using RentACar.DTOs.Coupon;
using RentACar.Entities;

namespace RentACar.Services;

public interface ICouponService
{
    Task<Coupon?> GetCouponById(int couponId);
    Task<Coupon?> GetCouponByCode(string code);
    Task<Coupon> CreateCoupon(CreateCouponDto createCouponDto, User user);
    Task<bool> ToggleCouponActiveState(int couponId);
    Task<bool> CheckIfCouponIsAlreadyReedemedByUser(string couponCode, int userId);
}