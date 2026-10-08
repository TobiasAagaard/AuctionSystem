namespace Auction.Avalonia.ViewModels;

using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Auction_Core.Models;
using Auction_Core.Repository;
using CommunityToolkit.Mvvm.Input;

public partial class HomeViewModel : ViewModelBase
{
    public ObservableCollection<Auction> UserAuctions { get; private set; } = new();
    public ObservableCollection<Auction> Auctions { get; private set; } = new();

    public Action? UserProfileRequested { get; set; }

    public Action? BidHistoryRequested { get; set; }

    private AuctionRepository _auctionRepo;
    private MainViewModel _mainViewModel;

    public HomeViewModel(MainViewModel mainViewModel, AuctionRepository auctionRepo)
    {
        _mainViewModel = mainViewModel;
        _auctionRepo = auctionRepo;
    }

    [RelayCommand]
    private void GoToUserProfile() => UserProfileRequested?.Invoke();

    [RelayCommand]
    private void GoToBidHistory() => BidHistoryRequested?.Invoke();

    public async void RefreshAuctionsAsync()
    {
        Auctions.Clear();
        UserAuctions.Clear();
        var allAuctions = await _auctionRepo.GetAllAuctionsAsync();
        
        foreach (var auction in allAuctions)
        {
            Auctions.Add(auction);
            if (_mainViewModel.CurrentUser == null)
                continue;

            if (auction.Seller.ID == _mainViewModel.CurrentUser.ID) //TODO: use the current user's ID instead of 1
            {
                UserAuctions.Add(auction);
            }
        }
    }
}