using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Auction.Avalonia.ViewModels;

public partial class LoginViewModel : ViewModelBase
{
    public Action? CreateUserRequested { get; set; }

    [ObservableProperty]
    public partial string Username { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Password { get; set; } = string.Empty;

    [RelayCommand]
    private void GoToCreateUser() => CreateUserRequested?.Invoke();
}
