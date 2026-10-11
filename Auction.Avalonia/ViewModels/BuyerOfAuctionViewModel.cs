using System;
using System.Threading.Tasks;
using Auction_Avalonia.Services;
using Auction_Core.Models;
using Auction_Core.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Auction_Avalonia.ViewModels;
public partial class BuyerOfAuctionViewModel : ViewModelBase
{
    private Auction_Core.Models.Auction _auction;
    private readonly IAuctionService _auctionService;
    private readonly SessionService _sessionService;
    private readonly ToastService _notifications;

    public Action? BackRequested { get; set; }

    [RelayCommand]
    private void GoBackToHome()
    {
        BackRequested?.Invoke();
    }

    public string ClosingString => $"Closing {DateTime.Now.ToShortDateString()}";

    [ObservableProperty]
    private string _currentBid;

    [ObservableProperty]
    private decimal? _bidAmount;

    // Highest bid on the auction, null when no bids have been placed yet
    private decimal? _highestBid;

    public BuyerOfAuctionViewModel(Auction_Core.Models.Auction auction, IAuctionService auctionService, SessionService sessionService, ToastService notifications)
    {
        _auction = auction;
        _auctionService = auctionService ?? throw new ArgumentNullException(nameof(auctionService), "Auction service cannot be null.");
        _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService), "Session service cannot be null.");
        _notifications = notifications ?? throw new ArgumentNullException(nameof(notifications), "Notifications service cannot be null.");

        _currentBid = $"DKK {_auction.CurrentBid}";
        _ = LoadCurrentBidAsync();
    }

    [RelayCommand]
    private async Task MakeBidAsync()
    {
        User? buyer = _sessionService.CurrentUser;

        if (buyer == null)
        {
            _notifications.Show("You must be logged in to make a bid", ToastViewModel.NotificationType.Warning);
            return;
        }

        string? error = ValidateBid(buyer);

        if (error != null)
        {
            _notifications.Show(error, ToastViewModel.NotificationType.Error);
            return;
        }

        decimal amount = BidAmount!.Value;
        bool accepted;

        try
        {
            accepted = await _auctionService.ReceiveBid(buyer, _auction.Id, amount);
        }
        catch (Exception)
        {
            _notifications.Show("Bid failed. Retry or contact support", ToastViewModel.NotificationType.Error);
            return;
        }
        await LoadCurrentBidAsync();

        if (!accepted)
        {
            _notifications.Show("Bid was not accepted. The auction may be closed or a higher bid was placed", ToastViewModel.NotificationType.Error);
            return;
        }

        _notifications.Show($"Bid of DKK {amount} placed successfully", ToastViewModel.NotificationType.Success);
        BidAmount = null;
    }

    private string? ValidateBid(User buyer)
    {
        if (BidAmount is not decimal amount) return "Bid amount is required";
        if (amount <= 0) return "Bid must be greater than zero";
        if (DateTime.UtcNow >= _auction.EndTime) return "The auction is closed";
        if (_highestBid is decimal highestBid && amount <= highestBid) return "Bid must be higher than the current bid";
        if (buyer.Balance < amount) return "Bid exceeds your balance";

        return null;
    }

    private async Task LoadCurrentBidAsync()
    {
        try
        {
            Bid? highestBid = await _auctionService.GetHighestBid(_auction.Id);
            _highestBid = highestBid?.Amount;
            CurrentBid = $"DKK {_highestBid ?? _auction.MinimumPrice}";
        }
        catch (Exception)
        {
            _notifications.Show("Could not load the current bid", ToastViewModel.NotificationType.Error);
        }
    }
}
