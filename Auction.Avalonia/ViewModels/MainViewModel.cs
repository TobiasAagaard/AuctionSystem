using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Auction_Avalonia.Services;
using Auction_Core.Repository;
using System;

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

    private readonly SetForSaleViewModel _setForSaleViewModel;

    private readonly UserProfileViewModel _userProfileViewModel;

    private readonly BidHistoryViewModel _bidHistoryViewModel;

    private readonly Func<Auction_Core.Models.Auction, BuyerOfAuctionViewModel> _buyerOfAuctionViewModelFactory;
    
    private SellerOfAuctionViewModel _sellerOfAuctionViewModel;
    private BuyerOfAuctionViewModel _buyerOfAuctionViewModel;

    public ToastService Notifications { get; }

    [ObservableProperty]
    public partial ViewModelBase CurrentPage { get; set; }

    public MainViewModel(LoginViewModel loginViewModel, 
        CreateUserViewModel createUserViewModel, 
        SetForSaleViewModel setForSaleViewModel, 
        HomeViewModel homeViewModel,
        UserProfileViewModel userProfileViewModel,
        SessionService sessionService, 
        ToastService notifications,
        Func<Auction_Core.Models.Auction, BuyerOfAuctionViewModel> buyerOfAuctionViewModelFactory)
    {
        _buyerOfAuctionViewModelFactory = buyerOfAuctionViewModelFactory;
        _loginViewModel = loginViewModel;
        _createUserViewModel = createUserViewModel;
        _homeViewModel = homeViewModel;
        _setForSaleViewModel = setForSaleViewModel;
        _sessionService = sessionService;
        Notifications = notifications;

        _bidHistoryViewModel = new(sessionService) { BackRequested = ShowHome };

        _userProfileViewModel = userProfileViewModel;
        _userProfileViewModel.BackRequested = ShowHome;
        

        _loginViewModel.CreateUserRequested = ShowCreateUser;
        _createUserViewModel.BackRequested = ShowLogin;
        _homeViewModel.GoToSetForSale = ShowSetForSale;
        _homeViewModel.UserProfileRequested = ShowUserProfile;
        _homeViewModel.BidHistoryRequested = ShowBidHistory;
        _homeViewModel.SellerOfAuctionRequested = ShowSellerOfAuction;
        _homeViewModel.BuyerOfAuctionRequested = ShowBuyerOfAuction;
        _setForSaleViewModel.BackRequested = ShowHome;

        CurrentPage = _loginViewModel;

        // Subscribe to the login successful event to update the current user
        _loginViewModel.LoginSuccessful = _ =>
        {
            _userProfileViewModel?.RefreshUserProfile();
            ShowHome();
        };

        _sessionService.LoggedOut += () =>
        {
            _loginViewModel.Reset();
            Notifications.Show("Logged out successfully", ToastViewModel.NotificationType.Success);
            ShowLogin();
        };
    }

    [RelayCommand]
    private void Logout()
    {
        if (!_sessionService.IsLoggedIn)
        {
            Notifications.Show("No user is currently logged in", ToastViewModel.NotificationType.Warning);
            return;
        }

        _sessionService.Logout();
    }

    private void ShowLogin() => CurrentPage = _loginViewModel;

    private void ShowCreateUser() => CurrentPage = _createUserViewModel;

    private void ShowSetForSale() => CurrentPage = _setForSaleViewModel;

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
        _buyerOfAuctionViewModel = _buyerOfAuctionViewModelFactory(selectedAuction);
        _buyerOfAuctionViewModel.BackRequested = ShowHome;

        CurrentPage = _buyerOfAuctionViewModel;
    }
}
