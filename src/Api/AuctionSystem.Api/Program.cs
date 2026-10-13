using AuctionSystem.Api.Extensions;
using AuctionSystem.Modules.Vehicles.Api;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);


builder.Services.AddOpenApi();
builder.Services.AddVehiclesModule(builder.Configuration);

WebApplication app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.ApplyMigrations();
}


await app.RunAsync();

