namespace AuctionSystem.Modules.Vehicles.Api.Vehicles;

public class Vehicle
{
    public int Id { get; set; }
    public string Vin { get; set; }
    public string Make { get; set; }
    public string Model { get; set; }
    public int Year { get; set; }
    public int Mileage { get; set; }
    public decimal BasePrice { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string Type { get; set; }
}