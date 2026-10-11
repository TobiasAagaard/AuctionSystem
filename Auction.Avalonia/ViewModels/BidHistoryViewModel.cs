using System;
using System.Collections.ObjectModel;
using Auction_Core.Models;
using Auction_Avalonia.Services;
using CommunityToolkit.Mvvm.Input;

namespace Auction_Avalonia.ViewModels;

public partial class BidHistoryViewModel : ViewModelBase
{

    private SessionService _sessionService;

    public ObservableCollection<Bid> UserBids { get; private set; } = new();

    public Action? BackRequested { get; set; }

    [RelayCommand]
    private void GoBackToHome()
    {
        BackRequested?.Invoke();
    }

    public BidHistoryViewModel(SessionService sessionService)
    {
        _sessionService = sessionService;
    }

    public async void RefreshBidHistoryAsync()
    {
        UserBids.Clear();
        // Not implemented
    }
}