using Auctionly.Modules.Auctions.Domain.ValueObjects;

namespace Auctionly.Modules.Auctions.Domain.Vehicles;


public class Vehicle
{
    public Vehicle()
    {
        
    }
 
    public int Id { get; private set; }
    public string Make { get; private set; }
    public string Model { get; private set; }
    public int Year { get; private set; }
    public int Mileage { get; private set; }
    public Money PurchasePrice { get; private set; }
    public double EngineSize { get; private set; }
    public VehicleType VehicleType  => VehicleDetails switch
    {
        PersonalCarSpecifications _ => VehicleType.PersonalCar,
        BusinessCarSpecifications _ => VehicleType.BusinessCar,
        _ => throw new InvalidOperationException("Unknown vehicle type.")
    };
    public FuelType FuelType { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }


    public VehicleSpecifications VehicleDetails { get; private set; }

       public Vehicle(string make, string model, int year, int mileage, Money purchasePrice, double engineSize, FuelType fuelType, VehicleSpecifications vehicleDetails)
    {

        if (fuelType == FuelType.Electric && engineSize != 0)
        {
            throw new ArgumentException("Electric vehicles should not have an engine size greater than zero.", nameof(engineSize));
        }
        else
        {
            if (fuelType != FuelType.Electric && engineSize <= 0)
            {
                throw new ArgumentException("Engine size must be greater than zero.", nameof(engineSize));
            }
        }

        if (year < 1886)
        {
            throw new ArgumentException("Year must be greater than or equal to 1886.", nameof(year));
        }

        Make = make;
        Model = model;
        Year = year;
        Mileage = mileage;
        PurchasePrice = purchasePrice;
        EngineSize = engineSize;
        FuelType = fuelType;
        VehicleDetails = vehicleDetails;
    }
}
