using Microsoft.EntityFrameworkCore;
using RentACar.Data;
using RentACar.DTOs;
using RentACar.DTOs.Coupon;
using RentACar.Entities;

namespace RentACar.Services;

public class CouponService : ICouponService
{
    private readonly AppDbContext _context;
    private readonly ILogger<CouponService> _logger;

    public CouponService(AppDbContext context, ILogger<CouponService> logger)
    {
        _context = context;
        _logger = logger;
    }
    
    public async Task<Coupon?> GetCouponById(int couponId)
    {
        return await _context.Coupons.FirstOrDefaultAsync(c => c.Id == couponId);
    }
    
    public async Task<Coupon?> GetCouponByCode(string code)
    {
        return await _context.Coupons.FirstOrDefaultAsync(c => c.Code == code);
    }

    public async Task<Result<Coupon>> CreateCoupon(CreateCouponDto createCouponDto, User user)
    {
        var exists = GetCouponByCode(createCouponDto.Code);
        if (exists is not null)
        {
            return Result<Coupon>.Failure(new Error("A coupon with this code already exists", "CouponAlreadyExists"));
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

        return Result<Coupon>.Success(coupon);
    }

    public async Task<Result<Coupon>> ToggleCouponActiveState(int couponId)
    {
        var coupon = await GetCouponById(couponId);
        if (coupon is null)
        {
            return Result<Coupon>.Failure(new Error("Coupon with this ID doesn't exists.", "InvalidCouponId"));
        }
        
        coupon.Active = !coupon.Active;
        coupon.UpdatedAt = DateTime.UtcNow;
        
        _context.Coupons.Update(coupon);
        int result = await _context.SaveChangesAsync();

        return result > 0 ? Result<Coupon>.Success(coupon) : Result<Coupon>.Failure(new Error("Failed to update the coupon's active state. Please try again later.", "UpdateFailed"));
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