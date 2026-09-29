using System.Collections.ObjectModel;
using Auction.Avalonia.Views;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Auction.Avalonia.ViewModels;

public partial class SetForSaleViewModel : ViewModelBase
{
    [ObservableProperty]
    public partial string? Name { get; set; } = string.Empty;
    [ObservableProperty]
    public partial string? Milage { get; set; } = string.Empty;
    [ObservableProperty]
    public partial string? RegNum { get; set; } = string.Empty;
    [ObservableProperty]
    public partial string? Year { get; set; } = string.Empty;
    [ObservableProperty]
    public partial string? EngineSize { get; set; } = string.Empty;
    [ObservableProperty]
    public partial string? Kmpl { get; set; } = string.Empty;
    [ObservableProperty]
    public partial bool? TowBar { get; set; }
    [ObservableProperty]
    public partial string? LicenseType { get; set; } = string.Empty;


    // License type selection
    [ObservableProperty]
    private string? _selectedLicenseType;
    public ObservableCollection<string> LicenseTypes { get; } = new()
    {
        "A",
        "B",
        "C",
        "D",
        "BE",
        "CE",
        "DE"
    };

    // Fuel type selection
    [ObservableProperty]
    private string? _selectedFuelType;
    public ObservableCollection<string> FuelTypes { get; } = new()
    {
        "Diesel",
        "Petrol",
        "Electric",
        "Hybrid"
    };
    
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
}
