using AuctionSystem.Api.Extensions;
using AuctionSystem.Modules.Auctions.Api;
using Scalar.AspNetCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);


builder.Services.AddOpenApi();
builder.Services.AddAuctionsModule(builder.Configuration);

WebApplication app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
    app.ApplyMigrations();
}


await app.RunAsync();

