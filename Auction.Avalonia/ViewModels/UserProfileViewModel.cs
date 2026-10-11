using System;
using Auction_Avalonia.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Auction_Avalonia.ViewModels;

public partial class UserProfileViewModel : ViewModelBase
{

    private readonly SessionService _sessionService;

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

    public UserProfileViewModel(SessionService sessionService)
    {
        _sessionService = sessionService;
    }

    public async void RefreshUserProfile()
    {
        Balance = _sessionService.CurrentUser?.Balance ?? 0;
        Username = _sessionService.CurrentUser?.Username ?? "Unknown";
    }
}