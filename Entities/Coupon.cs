using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace RentACar.Entities;

[Table("coupons")]
[Index(nameof(Code), IsUnique = true)]
public class Coupon
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }
    
    [Column("code")]
    public string Code { get; set; }
    
    [Column("description")]
    public string Description { get; set; }
    
    [Column("discount")]
    public float Discount { get; set; }
    
    [Column("active")]
    public bool Active { get; set; }
    
    [ForeignKey("User")]
    [Column("created_by")]
    public int CreatedBy { get; set; }
    
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
