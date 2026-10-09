using Auction_Core.Models;
using CommunityToolkit.Mvvm.Input;
using System;

namespace Auction_Avalonia.Services;

public class SessionService
{
    public User? CurrentUser { get; private set; }
    public bool IsLoggedIn => CurrentUser != null;

    public event Action? LoggedOut;

    public void Login(User user) => CurrentUser = user;

    public void Logout()
    {
        CurrentUser = null;
        LoggedOut?.Invoke();
    }
}