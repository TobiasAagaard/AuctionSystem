using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Auction_Core.Models;
using Auction.Avalonia.Services;
using Auction_Core.Repository;

namespace Auction.Avalonia.ViewModels;

/// <summary>
/// The Core view model. It holds whichever page is currently shown in MainWindow
/// and handles switching between pages.
/// </summary>
public partial class MainViewModel : ViewModelBase
{
    private readonly LoginViewModel _loginViewModel;
    private readonly CreateUserViewModel _createUserViewModel;

    private readonly HomeViewModel _homeViewModel;

    private SellerOfAuctionViewModel _sellerOfAuctionViewModel;

    public User? CurrentUser { get; private set; }
    public ToastService Notifications { get; }

    [ObservableProperty]
    public partial ViewModelBase CurrentPage { get; set; }

    public MainViewModel(LoginViewModel loginViewModel, CreateUserViewModel createUserViewModel, AuctionRepository auctionRepo, ToastService notifications)
    {
        _loginViewModel = loginViewModel;
        _createUserViewModel = createUserViewModel;
        Notifications = notifications;

        _loginViewModel.CreateUserRequested = ShowCreateUser;
        _createUserViewModel.BackRequested = ShowLogin;
        _homeViewModel = new(this, auctionRepo)
        {
            SellerOfAuctionRequested = ShowSellerOfAuction
        }; 

        CurrentPage = _loginViewModel;

        // Subscribe to the login successful event to update the current user
        _loginViewModel.LoginSuccessful = user =>
        {
            CurrentUser = user;
            ShowHome();
        };
    }

    [RelayCommand]
    private void Logout()
    {
        if (CurrentUser == null)
        {
            Notifications.Show("No user is currently logged in", ToastViewModel.NotificationType.Warning);
        }
        else
        {
            CurrentUser = null;
            _loginViewModel.Reset();
            Notifications.Show("Logged out successfully", ToastViewModel.NotificationType.Success);
            ShowLogin();
        }
    }

    private void ShowLogin() => CurrentPage = _loginViewModel;

    private void ShowCreateUser() => CurrentPage = _createUserViewModel;

    private void ShowHome() => CurrentPage = _homeViewModel;

    private void ShowSellerOfAuction(Auction_Core.Models.Auction selectedAuction) {
        _sellerOfAuctionViewModel = new(this, selectedAuction)
        {
            BackRequested = ShowHome
        };

        CurrentPage = _sellerOfAuctionViewModel;
    }
}
