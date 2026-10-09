using System;
using CommunityToolkit.Mvvm.Input;

namespace Auction.Avalonia.ViewModels;
public partial class BuyerOfAuctionViewModel : ViewModelBase
{
    private Auction_Core.Models.Auction _auction;
    private MainViewModel _mainViewModel;

    public Action? BackRequested { get; set; }

    [RelayCommand]
    private void GoBackToHome()
    {
        BackRequested?.Invoke();
    }

    public string ClosingString => $"Closing {DateTime.Now.ToShortDateString()}";

    public string CurrentBid => $"DKK {_auction.MinimumPrice}";

    public BuyerOfAuctionViewModel(MainViewModel mainViewModel, Auction_Core.Models.Auction auction)
    {
        _auction = auction;
        _mainViewModel = mainViewModel;
    }
}