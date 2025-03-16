using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RentACar.Enums;

namespace RentACar.Entities;

[Table("vehicles")]
public class Vehicle
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }
    
    [Column("make")]
    public string Make { get; set; }
    
    [Column("model")]
    public string Model { get; set; }
    
    [Column("year")]
    public int Year { get; set; }
    
    [Column("fuel_type")]
    public FuelType FuelType { get; set; }
    
    [Column("color")]
    public string Color { get; set; }
    
    [Column("price_per_day")]
    public float PricePerDay { get; set; }
    
    [Column("is_available")]
    public bool IsAvailable { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}