using AuctionSystem.Api.Extensions;
using AuctionSystem.Modules.Vehicles.Api;
using Scalar.AspNetCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);


builder.Services.AddOpenApi();
builder.Services.AddVehiclesModule(builder.Configuration);

WebApplication app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
    app.ApplyMigrations();
}


await app.RunAsync();

