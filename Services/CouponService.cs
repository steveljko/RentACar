using Microsoft.EntityFrameworkCore;
using RentACar.Data;
using RentACar.DTOs.Coupon;
using RentACar.Entities;

namespace RentACar.Services;

public class CouponService : ICouponService
{
    private readonly AppDbContext _context;

    public CouponService(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<Coupon?> GetCouponById(int couponId)
    {
        return await _context.Coupons.FirstOrDefaultAsync(c => c.Id == couponId);
    }
    
    public async Task<Coupon?> GetCouponByCode(string code)
    {
        return await _context.Coupons.FirstOrDefaultAsync(c => c.Code == code);
    }

    public async Task<Coupon> CreateCoupon(CreateCouponDto createCouponDto, User user)
    {
        var exists = GetCouponByCode(createCouponDto.Code);
        if (exists != null)
        {
            throw new InvalidOperationException("A coupon with this code already exists.");
        }
        
        var coupon = new Coupon
        {
            Code = createCouponDto.Code,
            Description = createCouponDto.Description,
            Discount = createCouponDto.Discount,
            Active = true,
            CreatedBy = user.Id,
        };

        await _context.Coupons.AddAsync(coupon);
        await _context.SaveChangesAsync();

        return coupon;
    }

    public async Task<bool> ToggleCouponActiveState(int couponId)
    {
        var coupon = await GetCouponById(couponId);
        if (coupon is null)
        {
            return false;
        }
        
        coupon.Active = !coupon.Active;
        coupon.UpdatedAt = DateTime.UtcNow;
        
        _context.Coupons.Update(coupon);
        int result = await _context.SaveChangesAsync();

        return result > 0;
    }
    
    public async Task<bool> CheckIfCouponIsAlreadyReedemedByUser(string couponCode, int userId)
    {
        var coupon = await GetCouponByCode(couponCode);
        if (coupon is null)
        {
            return false;
        }
        
        return await _context.CouponRedemptions
            .AnyAsync(cr => cr.CouponId == coupon.Id && cr.UserId == userId);
    }
}