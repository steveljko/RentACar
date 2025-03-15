namespace RentACar.DTOs.Vehicle;

public class VehicleDto
{
    public string Make { get; set; }
    public string Model { get; set; }
    public int Year { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}