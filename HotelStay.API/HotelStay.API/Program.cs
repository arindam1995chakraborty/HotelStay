using HotelStay.API.Controllers;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// We use Minimal APIs (no MVC controllers) per assignment requirements.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS - allow local frontend during development
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost", policy =>
    {
        policy.WithOrigins(
            "http://localhost:4200",
            "https://localhost:4200",
            "http://localhost:5173",
            "https://localhost:5173"
        )
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

// register providers and in-memory store and services
builder.Services.AddSingleton<HotelStay.API.Stores.InMemoryReservationStore>();
builder.Services.AddScoped<HotelStay.API.Providers.IHotelProvider, HotelStay.API.Providers.PremierStaysProvider>();
builder.Services.AddScoped<HotelStay.API.Providers.IHotelProvider, HotelStay.API.Providers.BudgetNestsProvider>();
builder.Services.AddScoped<HotelStay.API.Services.IHotelService, HotelStay.API.Services.HotelService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Use CORS policy
app.UseCors("AllowLocalhost");

// Minimal API endpoints
// GET /hotels/search
app.MapGet("/hotels/search", HotelsController.Search);

// POST /hotels/reserve
app.MapPost("/hotels/reserve", HotelsController.Reserve);

// GET /hotels/reservation/{reference}
app.MapGet("/hotels/reservation/{reference}", HotelsController.GetReservation);

app.Run();
