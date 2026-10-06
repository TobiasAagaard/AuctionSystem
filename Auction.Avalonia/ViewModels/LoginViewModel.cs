using System;
using System.Threading.Tasks;
using Auction_Core.Models;
using Auction.Avalonia.Services;
using Auction_Core.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Linq;

namespace Auction.Avalonia.ViewModels;

public partial class LoginViewModel : ViewModelBase
{
    private readonly IAuthService _authService;

    public ToastService Notification { get; }

    public Action? CreateUserRequested { get; set; }

    public Action<User>? LoginSuccessful { get; set; }

    [ObservableProperty]
    public partial string Username { get; set; } = string.Empty;

    [ObservableProperty]
    public partial string Password { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool IsLoggedIn { get; set; } = false;

    public LoginViewModel(IAuthService authService, ToastService notification)
    {
        this._authService = authService;
        this.Notification = notification;
    }

    [RelayCommand]
    private void GoToCreateUser()
    {
        Username = string.Empty;
        Password = string.Empty;
        CreateUserRequested?.Invoke();
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        IsLoggedIn = false;

        try
        {
            var user = await _authService.AuthenticateAsync(Username, Password);
            if (user != null)
            {
                IsLoggedIn = true;
                Username = string.Empty;
                Password = string.Empty;
                LoginSuccessful?.Invoke(user);

                Notification.Show("Login successful", ToastViewModel.NotificationType.Success);
            }
        }
        catch (Exception ex)
        {
            Notification.Show(ex.Message, ToastViewModel.NotificationType.Error);
        }
    }
}
