using System;
using System.Threading.Tasks;
using Auction_Avalonia.Services;
using Auction_Core.Models;
using Auction_Core.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Auction_Avalonia.ViewModels;

public partial class UserProfileViewModel : ViewModelBase
{

    private readonly SessionService _sessionService;
    private readonly IUserService _userService;
    private readonly ToastService _notifications;

    public Action? BackRequested { get; set; }

    [RelayCommand]
    private void GoBackToHome()
    {
        BackRequested?.Invoke();
    }

    [ObservableProperty]
    private string _username;

    [ObservableProperty]
    private decimal _balance;

    [ObservableProperty]
    private decimal? _depositAmount;

    public UserProfileViewModel(SessionService sessionService, IUserService userService, ToastService notifications)
    {
        _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService), "Session service cannot be null.");
        _userService = userService ?? throw new ArgumentNullException(nameof(userService), "User service cannot be null.");
        _notifications = notifications ?? throw new ArgumentNullException(nameof(notifications), "Notifications service cannot be null.");
    }

    [RelayCommand]
    private async Task DepositAsync()
    {
        User? user = _sessionService.CurrentUser;

        if (user == null)
        {
            _notifications.Show("You must be logged in to deposit money", ToastViewModel.NotificationType.Warning);
            return;
        }

        if (DepositAmount is not decimal amount || amount <= 0)
        {
            _notifications.Show("Deposit must be greater than zero", ToastViewModel.NotificationType.Error);
            return;
        }

        try
        {
            await _userService.DepositAsync(user, amount);
        }
        catch (Exception)
        {
            _notifications.Show("Deposit failed. Retry or contact support", ToastViewModel.NotificationType.Error);
            return;
        }

        _notifications.Show($"Deposited DKK {amount} successfully", ToastViewModel.NotificationType.Success);
        DepositAmount = null;
        RefreshUserProfile();
    }

    public async void RefreshUserProfile()
    {
        Balance = _sessionService.CurrentUser?.Balance ?? 0;
        Username = _sessionService.CurrentUser?.Username ?? "Unknown";
    }
}