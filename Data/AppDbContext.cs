using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RentACar.Entities;
using RentACar.Enums;

namespace RentACar.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions options): base(options)
    {}
    
    public DbSet<User> Users { get; set; }
}