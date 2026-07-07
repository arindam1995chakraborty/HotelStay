var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

app.UseAuthorization();

app.MapControllers();

app.Run();
