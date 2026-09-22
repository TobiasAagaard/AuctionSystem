using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Auction.Avalonia.ViewModels;

public partial class CreateUserViewModel : ViewModelBase
{
    public Action? BackRequested { get; set; }
    public Action? CreateUserRequested { get; set; }

    [ObservableProperty]
    public partial string Username { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Password { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string PasswordAgain {get; set; } = string.Empty;


    [RelayCommand]
    private void GoBackToLogin()
    {
        BackRequested?.Invoke();
    }

    [RelayCommand]
    private void CreateUser() 
    {
        CreateUserRequested?.Invoke();
    }
}
