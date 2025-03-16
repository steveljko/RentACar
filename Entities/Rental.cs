using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RentACar.Entities;

[Table("rentals")]
public class Rental
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }

    [ForeignKey("Vehicle")]
    [Column("vehicle_id")]
    public int VehicleId { get; set; }

    [Column("start_date")]
    public DateTime StartDate { get; set; }

    [Column("end_date")]
    public DateTime EndDate { get; set; }
    
    [Column("total_price")]
    public float TotalPrice { get; set; }
    
    [Column("rented_by")]
    public int RentedBy { get; set; }

    public virtual Vehicle Vehicle { get; set; }
    
    [ForeignKey("RentedBy")]
    public virtual User Renter { get; set; }
}