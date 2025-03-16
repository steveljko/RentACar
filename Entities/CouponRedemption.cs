using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RentACar.Entities;


[Table("coupon_redemptions")]
public class CouponRedemption
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }
    
    [ForeignKey("")]
    [Column("rental_id")]
    public int RentalId { get; set; }
    
    [Column("coupon_id")]
    public int CouponId { get; set; }
    
    [Column("user_id")]
    public int UserId { get; set; }
    
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}