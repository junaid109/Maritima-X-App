var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/telemetry", () =>
{
    // Mock Data Generator
    var now = DateTime.UtcNow;
    var rng = Random.Shared;
    
    var ships = new List<MaritimaX.Core.Models.ShipTelemetry>
    {
        new() {
            ShipId = "IMO-9812345",
            Name = "Pacific Titan",
            Latitude = 34.0522 + (rng.NextDouble() * 0.01),
            Longitude = -118.2437 + (rng.NextDouble() * 0.01),
            Heading = 270,
            SpeedKnots = 18.5,
            EngineRpm = 1200 + rng.Next(-50, 50),
            EngineTempC = 85.5 + rng.NextDouble(),
            FuelLevelPercent = 78.0,
            IsEmergency = false,
            Timestamp = now
        },
        new() {
            ShipId = "IMO-7654321",
            Name = "Atlantic Voyager",
            Latitude = 40.7128 + (rng.NextDouble() * 0.01),
            Longitude = -74.0060 + (rng.NextDouble() * 0.01),
            Heading = 90,
            SpeedKnots = 12.0,
            EngineRpm = 1100 + rng.Next(-50, 50),
            EngineTempC = 400.0, // Overheating!
            FuelLevelPercent = 45.2,
            IsEmergency = true, // Simulation of issue
            Timestamp = now
        }
    };
    
    return ships;
})
.WithName("GetTelemetry");

app.MapDefaultEndpoints();

app.Run();
