using System;
using System.Collections.ObjectModel;
using Auction_Avalonia.Views;
using Auction_Core.Services;
using Auction_Avalonia.Services;
using Auction_Core.Repository;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using Auction_Core.Models;
using Auction_Core.Enums;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls.Notifications;

namespace Auction_Avalonia.ViewModels;

public partial class SetForSaleViewModel : ViewModelBase
{
    private readonly SessionService _sessionService;
    private readonly IAuctionService _auctionService;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly ToastService _notifications;

        public SetForSaleViewModel(IAuctionService auctionService, IVehicleRepository vehicleRepository, SessionService sessionService, ToastService notifications)
    {
        _auctionService = auctionService ?? throw new ArgumentNullException(nameof(auctionService), "Auction service cannot be null.");
        _vehicleRepository = vehicleRepository ?? throw new ArgumentNullException(nameof(vehicleRepository), "Vehicle repository cannot be null.");
        _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService), "Session service cannot be null.");
        _notifications = notifications ?? throw new ArgumentNullException(nameof(notifications), "Notifications service cannot be null.");
    }

    public Action? BackRequested { get; set; }
    
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

    [RelayCommand]
    private void Cancel()
    {
        ResetFildes();
        BackRequested?.Invoke();
    }

    [RelayCommand]
    private async Task CreateAuctionAsync()
    {
        User seller = _sessionService.CurrentUser;

        if (seller == null)
        {
            _notifications.Show("You must be logged in to create a auction", ToastViewModel.NotificationType.Warning);
            return;
        }

        string? error = ValidateAuction();

        if (error != null)
        {
            _notifications.Show(error, ToastViewModel.NotificationType.Error);
            return;
        }

        try
        {
            Vehicle vehicle = BuildVehicle();
            await _vehicleRepository.AddVehicleAsync(vehicle);
            await _auctionService.SetForSale(vehicle, seller, StartingBid, CloseAuctionDate);
        }
        catch (Exception ex)
        {
            _notifications.Show("Auction creation failed. Retry or contact support", ToastViewModel.NotificationType.Error);
            BackRequested?.Invoke();
            return;
        }

        _notifications.Show("Auction created successfully", ToastViewModel.NotificationType.Success);
        ResetFildes();
        BackRequested?.Invoke();

    }

    private string? ValidateAuction()
    {
        if (string.IsNullOrWhiteSpace(SelectedVehicleType)) return "Vehicle type is required";
        if (string.IsNullOrWhiteSpace(Name)) return "Vehicle name is required";
        if (string.IsNullOrWhiteSpace(RegNum)) return "Registration number is required";
        if (StartingBid <= 0) return "Starting bid must be greater than zero";
        if (CloseAuctionDate <= DateTime.Now) return "Close auction date must be in the future";

        return null;
    }

    private Vehicle BuildVehicle() => SelectedVehicleType switch
    {
        "Business Personal Car" => new BusinessPersonalCar(0, Name!, Milage, RegNum!, Year, BasePrice, EngineSize, Kmpl,
                                       SelectedFuelType, SeatCount, RollCage, CargoCapacity),
        "Private Personal Car" => new PrivatePersonalCar(0, Name!, Milage, RegNum!, Year, BasePrice,TowBar, EngineSize, Kmpl,
                                       SelectedFuelType, SeatCount, RollCage),
        "Semi Truck" => new SemiTruck(0, Name!, Milage, RegNum!, Year, BasePrice, TowBar, EngineSize, Kmpl, CargoCapacity,
                                       Height, Weight, Length),
        "Bus" => new Bus(0, Name!, Milage, RegNum!, Year, BasePrice, TowBar, EngineSize, Kmpl, Weight, Height, Length, SeatCount, BedCount, Toilet),
        _ => throw new InvalidOperationException("Unsupported vehicle type")
    };

    private void ResetFildes()
    {
        Name = string.Empty;
        RegNum = string.Empty;
        Milage = 0;
        Year = 0;
        StartingBid = 0;
        BasePrice = 0;
        EngineSize = 0;
        Kmpl = 0;
        TowBar = false;
        CloseAuctionDate = DateTime.Now.AddDays(7);
        Weight = 0;
        Height = 0;
        Length = 0;
        Toilet = false;
        BedCount = 0;
        Isofix = false;
        RollCage = false;
        SeatCount = 0;
        CargoCapacity = 0;
        SelectedFuelType = FuelTypes.FirstOrDefault();
        SelectedVehicleType = VehicleTypes.FirstOrDefault();
    }
    
}