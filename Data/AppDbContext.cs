using Microsoft.EntityFrameworkCore;
using RentACar.Entities;

namespace RentACar.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions options): base(options)
    {}
    
    public DbSet<User> Users { get; set; }
    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<Rental> Rentals { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Rental>()
            .HasOne(r => r.Vehicle)
            .WithMany()
            .HasForeignKey(r => r.VehicleId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<Rental>()
            .HasOne(r => r.Renter)
            .WithMany()
            .HasForeignKey(r => r.RentedBy)
            .OnDelete(DeleteBehavior.Cascade);
    }
}