using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Auction_Avalonia.ViewModels;

public partial class UserProfileViewModel : ViewModelBase
{

    private readonly MainViewModel _mainViewModel;

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

    public UserProfileViewModel(MainViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
    }

    public async void RefreshUserProfile()
    {
        Balance = _mainViewModel.CurrentUser?.Balance ?? 0;
        Username = _mainViewModel.CurrentUser?.Username ?? "Unknown";
    }
}