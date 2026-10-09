using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Auction_Core.Models;
using Auction_Avalonia.Services;
using Auction_Avalonia.Services;
using Auction_Core.Repository;

namespace Auction_Avalonia.ViewModels;

/// <summary>
/// The Core view model. It holds whichever page is currently shown in MainWindow
/// and handles switching between pages.
/// </summary>
public partial class MainViewModel : ViewModelBase
{
    private readonly LoginViewModel _loginViewModel;

    private readonly CreateUserViewModel _createUserViewModel;

    private readonly HomeViewModel _homeViewModel;
    
    private readonly SessionService _sessionService;

    private readonly UserProfileViewModel _userProfileViewModel;

    private readonly BidHistoryViewModel _bidHistoryViewModel;
    
    private SellerOfAuctionViewModel _sellerOfAuctionViewModel;
    private BuyerOfAuctionViewModel _buyerOfAuctionViewModel;

    private readonly HomeViewModel  _homeViewModel;

    public User? CurrentUser { get; private set; }
    public ToastService Notifications { get; }

    [ObservableProperty]
    public partial ViewModelBase CurrentPage { get; set; }

    public MainViewModel(LoginViewModel loginViewModel, 
        CreateUserViewModel createUserViewModel, 
        SetForSaleViewModel setForSaleViewModel, 
        HomeViewModel homeViewModel,
        SessionService sessionService, 
        ToastService notifications)
    {
        _loginViewModel = loginViewModel;
        _createUserViewModel = createUserViewModel;
        _homeViewModel = homeViewModel;
        _setForSaleViewModel = setForSaleViewModel;
        Notifications = notifications;
        _sessionService = sessionService;

        _loginViewModel.CreateUserRequested = ShowCreateUser;
        _createUserViewModel.BackRequested = ShowLogin;
        _homeViewModel = new(this, auctionRepo)
        {
            UserProfileRequested = ShowUserProfile,
            BidHistoryRequested = ShowBidHistory,
            SellerOfAuctionRequested = ShowSellerOfAuction,
            BuyerOfAuctionRequested = ShowBuyerOfAuction
        };

        _userProfileViewModel = new(this)
        {
            BackRequested = ShowHome
        };
        _bidHistoryViewModel = new(this)
        {
            BackRequested = ShowHome
        };

        _userProfileViewModel = new(this)
        {
            BackRequested = ShowHome
        };

        CurrentPage = _loginViewModel;

        // Subscribe to the login successful event to update the current user
        _loginViewModel.LoginSuccessful = _ =>
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
            _sessionService.Logout();
            _loginViewModel.Reset();
            Notifications.Show("Logged out successfully", ToastViewModel.NotificationType.Success);
            ShowLogin();
        }
    }

    private void ShowLogin() => CurrentPage = _loginViewModel;

    private void ShowCreateUser() => CurrentPage = _createUserViewModel;

    private void ShowHome() => CurrentPage = _homeViewModel;

    private void ShowUserProfile() => CurrentPage = _userProfileViewModel;

    private void ShowBidHistory() => CurrentPage = _bidHistoryViewModel;
    
    private void ShowSellerOfAuction(Auction_Core.Models.Auction selectedAuction) {
        _sellerOfAuctionViewModel = new(this, selectedAuction)
        {
            BackRequested = ShowHome
        };

        CurrentPage = _sellerOfAuctionViewModel;
    }

    private void ShowBuyerOfAuction(Auction_Core.Models.Auction selectedAuction) {
        _buyerOfAuctionViewModel = new(this, selectedAuction)
        {
            BackRequested = ShowHome
        };

        CurrentPage = _buyerOfAuctionViewModel;
    }
}
