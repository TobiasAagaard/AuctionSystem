using System;
using System.Threading.Tasks;
using Auction_Core.Models;
using Auction.Avalonia.Services;
using Auction_Core.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Linq;

namespace Auction.Avalonia.ViewModels;

public partial class CreateUserViewModel : ViewModelBase
{
    private readonly IAuthService _authService;

    public ToastService Notification { get; }

    public Action? BackRequested { get; set; }
    public Action? CreateUserRequest { get; set; }

    [ObservableProperty] 
    public partial string Username { get; set; } = string.Empty;

    [ObservableProperty] 
    public partial string Password { get; set; } = string.Empty;

    [ObservableProperty] 
    public partial string PasswordAgain {get; set; } = string.Empty;
    [ObservableProperty]
    public partial string PostalCode { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool IsCreatingUser { get; set; } = false;

    public CreateUserViewModel(IAuthService authService, ToastService notification)
    {
        this._authService = authService;
        this.Notification = notification;
    }

    [RelayCommand]
    private void GoBackToLogin()
    {
        BackRequested?.Invoke();
    }

    [RelayCommand]
    private async Task CreateUserAsync()
    {
        if (!ValidateInput())
        {
            return;
        }

        IsCreatingUser = true;

        try
        {
            User user = await _authService.RegisterAsync(Username, Password, PostalCode);
            Password = PasswordAgain = string.Empty;
            Username = string.Empty;
            PostalCode = string.Empty;

            CreateUserRequest?.Invoke();

            Notification.Show("User created successfully", ToastViewModel.NotificationType.Success);
        } 
        catch (Exception ex)
        {
            Notification.Show(ex.Message, ToastViewModel.NotificationType.Error);
        }
        finally
        {
            IsCreatingUser = false;
        }
    }

    private bool ValidateInput()
    {
        if (string.IsNullOrWhiteSpace(Username) ||
            string.IsNullOrWhiteSpace(Password) ||
            string.IsNullOrWhiteSpace(PasswordAgain) ||
            string.IsNullOrWhiteSpace(PostalCode))
        {
                Notification.Show("All fields are required", ToastViewModel.NotificationType.Error);
            return false;
        }

        if (Password != PasswordAgain)
        {
                Notification.Show("Passwords do not match", ToastViewModel.NotificationType.Error);
            return false;
        }

            if (Password.Length < 8)
        {
                Notification.Show("Password must be at least 8 characters long", ToastViewModel.NotificationType.Error);
            return false;
        }

        if (PostalCode.Length != 4 || !PostalCode.All(char.IsDigit))
        {
                Notification.Show("Postal code must be a valid four-digit number", ToastViewModel.NotificationType.Error);
            return false;
        }

        return true;
    }
}
