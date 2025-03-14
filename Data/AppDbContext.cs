using Microsoft.EntityFrameworkCore;
using RentACar.Entities;

namespace RentACar.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions options): base(options)
    {}
    
    public DbSet<User> Users { get; set; }
    public DbSet<Vehicle> Vehicles { get; set; }
}