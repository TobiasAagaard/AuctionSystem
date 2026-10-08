using System;
using System.Collections.ObjectModel;
using Auction.Avalonia.Views;
using Auction_Core.Services;
using Auction_Core.Repository;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using Auction_Core.Models;
using Auction_Core.Enums;

namespace Auction.Avalonia.ViewModels;

public partial class SetForSaleViewModel : ViewModelBase
{
    public SetForSaleViewModel(IAuctionService auctionService, IVehicleRepository vehicleRepository, MainViewModel mainViewModel)
    {
        _auctionService = auctionService ?? throw new ArgumentNullException(nameof(auctionService), "Auction service cannot be null.");
        _vehicleRepository = vehicleRepository ?? throw new ArgumentNullException(nameof(vehicleRepository), "Vehicle repository cannot be null.");
        _mainViewModel = mainViewModel ?? throw new ArgumentNullException(nameof(mainViewModel), "Main view model cannot be null.");
    }
    private readonly IAuctionService _auctionService;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly MainViewModel _mainViewModel;
    
    // Vehicle properties
    [ObservableProperty]
    private string? _name = string.Empty;
    [ObservableProperty]
    private  double _milage;
    [ObservableProperty]
    private  string? _regNum = string.Empty;
    [ObservableProperty]
    private  int _year;
    [ObservableProperty]
    private  decimal _startingBid;
    [ObservableProperty]
    private  double _basePrice;
    [ObservableProperty]
    private  double _engineSize;
    [ObservableProperty]
    private  double _kmpl;
    [ObservableProperty]
    private  bool _towBar = false;
    [ObservableProperty]
    private DateTime _closeAuctionDate = DateTime.Now.AddDays(7); // Default to 7 days from now

    // Heavy vehicle properties
    [ObservableProperty]
    private  double _weight;
    [ObservableProperty]
    private  double _height;
    [ObservableProperty]
    private  double _length;


    // Bus properties
    [ObservableProperty]
    private  bool _toilet;
    [ObservableProperty]
    private  int _bedCount;

    // PrivatePersonalCar properties
    [ObservableProperty]
    private  bool _isofix;

    // BusinessPersonalCar properties
    [ObservableProperty]
    private  bool _rollCage;


    // other properties    
    [ObservableProperty]
    private  int _seatCount;
    [ObservableProperty]
    private  double _cargoCapacity;

    // Fuel type selection
    [ObservableProperty]
    private FuelType _selectedFuelType;
    public ObservableCollection<FuelType> FuelTypes { get; } = new(Enum.GetValues<FuelType>());
    
    // Vehicle type selection
    [ObservableProperty]
    private string? _selectedVehicleType;

    [ObservableProperty]
    private UserControl? _selectedVehicleView;

    public ObservableCollection<string> VehicleTypes { get; } = new()
    {
        "Business Personal Car",
        "Private Personal Car",
        "Semi Truck",
        "Bus"
    };
    partial void OnSelectedVehicleTypeChanged(string? value)
    {
        SelectedVehicleView = value switch
        {
            "Business Personal Car" => new BusinessPersonalCarView(),
            "Private Personal Car" => new PrivatePersonalCarView(),
            "Semi Truck" => new SemiTruckView(),
            "Bus" => new BusView(),
            _ => null
        };
    }
    
    public async void CreateAuction()
    {
        // Here you would create the auction based on the selected vehicle type and its properties.
        // This is just a placeholder for demonstration purposes.
        if (string.IsNullOrEmpty(SelectedVehicleType)) throw new InvalidOperationException("No vehicle type selected.");
        
        if (CloseAuctionDate < DateTime.Now) throw new InvalidOperationException("Close auction date cannot be in the past.");
        
        if (StartingBid <= 0) throw new InvalidOperationException("Starting bid must be greater than zero.");

        if (string.IsNullOrEmpty(Name)) throw new InvalidOperationException("Vehicle name cannot be empty.");
        if (string.IsNullOrEmpty(RegNum)) throw new InvalidOperationException("Vehicle registration number cannot be empty.");
        
        switch (SelectedVehicleType)
        {
            case "Business Personal Car":
                // Create a Business Personal Car auction
                var businessCar = new BusinessPersonalCar
                (
                    0,
                    Name,
                    Milage,
                    RegNum,
                    Year,
                    BasePrice,
                    EngineSize,
                    Kmpl,
                    SelectedFuelType,
                    SeatCount,
                    RollCage,
                    CargoCapacity
                );
                await _vehicleRepository.AddVehicleAsync(businessCar);
                await _auctionService.SetForSale(businessCar, _mainViewModel.CurrentUser, StartingBid, CloseAuctionDate);
                break;
            case "Private Personal Car":
                // Create a Private Personal Car auction
                var privateCar = new PrivatePersonalCar
                (
                    0,
                    Name,
                    Milage,
                    RegNum,
                    Year,
                    BasePrice,
                    TowBar,
                    EngineSize,
                    Kmpl,
                    SelectedFuelType,
                    SeatCount,
                    Isofix
                );
                await _vehicleRepository.AddVehicleAsync(privateCar);
                await _auctionService.SetForSale(privateCar, _mainViewModel.CurrentUser, StartingBid, CloseAuctionDate);
                break;
            case "Semi Truck":
                // Create a Semi Truck auction
                var semiTruck = new SemiTruck
                (
                    0,
                    Name,
                    Milage,
                    RegNum,
                    Year,
                    BasePrice,
                    TowBar,
                    EngineSize,
                    Kmpl,
                    CargoCapacity,
                    Weight,
                    Height,
                    Length
                );
                await _vehicleRepository.AddVehicleAsync(semiTruck);
                await _auctionService.SetForSale(semiTruck, _mainViewModel.CurrentUser, StartingBid, CloseAuctionDate);
                break;
            case "Bus":
                // Create a Bus auction
                var bus = new Bus
                (
                    0,
                    Name,
                    Milage,
                    RegNum,
                    Year,
                    BasePrice,
                    TowBar,
                    EngineSize,
                    Kmpl,
                    Weight,
                    Height,
                    Length,
                    SeatCount,
                    BedCount,
                    Toilet
                );
                await _vehicleRepository.AddVehicleAsync(bus);
                await _auctionService.SetForSale(bus, _mainViewModel.CurrentUser, StartingBid, CloseAuctionDate);
                break;
            default:
                throw new InvalidOperationException("Unknown vehicle type selected.");
        }


    }
}
