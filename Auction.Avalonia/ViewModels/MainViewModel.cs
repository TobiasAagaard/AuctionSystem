using CommunityToolkit.Mvvm.ComponentModel;
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
    private readonly SetForSaleViewModel _setForSaleViewModel;

    public User? CurrentUser { get; private set; }
    public ToastService Notifications { get; }

    [ObservableProperty]
    public partial ViewModelBase CurrentPage { get; set; }

    public MainViewModel(LoginViewModel loginViewModel, CreateUserViewModel createUserViewModel, SetForSaleViewModel setForSaleViewModel, AuctionRepository auctionRepo, ToastService notifications)
    {
        _loginViewModel = loginViewModel;
        _createUserViewModel = createUserViewModel;
        _setForSaleViewModel = setForSaleViewModel;
        Notifications = notifications;

        _loginViewModel.CreateUserRequested = ShowCreateUser;
        _createUserViewModel.BackRequested = ShowLogin;
        _homeViewModel = new(this, auctionRepo); 

        CurrentPage = _loginViewModel;

        // Subscribe to the login successful event to update the current user
        _loginViewModel.LoginSuccessful = user =>
        {
            CurrentUser = user;
            CurrentPage = _homeViewModel;
        };
    }

    private void ShowLogin() => CurrentPage = _loginViewModel;

    private void ShowCreateUser() => CurrentPage = _createUserViewModel;
}
