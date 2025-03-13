using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using RentACar.Data;
using RentACar.Enums;
using RentACar.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo()
    {
        Version = "v1",
        Title = "Rent A Car API",
        Description = "The Rent A Car API enables users to browse available vehicles and make rentals."
    });
});

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options
        .UseNpgsql(builder.Configuration.GetConnectionString("DbConnection"), o =>
        {
            // map enum types to their corresponding postgresql enum types
            o.MapEnum<UserRole>("user_role");
        })
        .LogTo(Console.WriteLine);
});

builder.Services.AddControllers();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();