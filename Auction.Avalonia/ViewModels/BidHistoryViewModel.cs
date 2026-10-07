using System;
using System.Collections.ObjectModel;
using Auction_Core.Models;
using CommunityToolkit.Mvvm.Input;

namespace Auction.Avalonia.ViewModels;

public partial class BidHistoryViewModel : ViewModelBase
{

    private MainViewModel _mainViewModel;
    public ObservableCollection<Bid> UserBids { get; private set; } = new();

    public Action? BackRequested { get; set; }

    [RelayCommand]
    private void GoBackToHome()
    {
        BackRequested?.Invoke();
    }

    public BidHistoryViewModel(MainViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
    }

    public async void RefreshBidHistoryAsync()
    {
        UserBids.Clear();
        // Not implemented
    }
}