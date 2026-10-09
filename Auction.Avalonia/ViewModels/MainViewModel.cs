using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Auction_Avalonia.Services;
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

    private readonly SessionService _sessionService;

    private readonly SetForSaleViewModel _setForSaleViewModel;
    private readonly HomeViewModel  _homeViewModel;

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
        _homeViewModel.GoToSetForSale = ShowSetForSale;


        CurrentPage = _loginViewModel;

        // Subscribe to the login successful event to update the current user
        _loginViewModel.LoginSuccessful = _ =>
        {
            CurrentPage = _homeViewModel;
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
}
