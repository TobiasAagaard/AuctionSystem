using System;
using System.Threading.Tasks;
using Auction_Core.Models;
using Auction_Core.Services;
using Auction_Core.Repository;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Npgsql;
using System.Linq;

namespace Auction.Avalonia.ViewModels;

public partial class CreateUserViewModel : ViewModelBase
{
    private readonly IAuthService _authService;

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
    public partial string ErrorMessage { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool IsCreatingUser { get; set; } = false;

    public CreateUserViewModel(IAuthService authService)
    {
        this._authService = authService;
    }

    [RelayCommand]
    private void GoBackToLogin()
    {
        BackRequested?.Invoke();
    }

    [RelayCommand]
    private async Task CreateUserAsync()
    {
        ErrorMessage = string.Empty;

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
        } 
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            
            IsCreatingUser = false;
            if (string.IsNullOrEmpty(ErrorMessage))
            {
                GoBackToLogin();
            }
        }
    }

    private bool ValidateInput()
    {
        if (string.IsNullOrWhiteSpace(Username) ||
            string.IsNullOrWhiteSpace(Password) ||
            string.IsNullOrWhiteSpace(PasswordAgain) ||
            string.IsNullOrWhiteSpace(PostalCode))
        {
            ErrorMessage = "All fields are required";
            return false;
        }

        if (Password != PasswordAgain)
        {
            ErrorMessage = "Passwords do not match";
            return false;
        }

        if (Password.Length < 9)
        {
            ErrorMessage = "Password must be longer than 8 characters";
            return false;
        }

        if (PostalCode.Length != 4 || !PostalCode.All(char.IsDigit))
        {
            ErrorMessage = "Postal code must be a valid four-digit number";
            return false;
        }

        return true;
    }
}
