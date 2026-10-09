namespace Auction_Avalonia.ViewModels;

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Auction_Avalonia.Services;
using CommunityToolkit.Mvvm.Input;
using System;
using Auction_Core.Models;
using Auction_Core.Repository;
using CommunityToolkit.Mvvm.Input;

public partial class HomeViewModel : ViewModelBase
{
    public ObservableCollection<Auction> UserAuctions { get; private set; } = new();
    public ObservableCollection<Auction> Auctions { get; private set; } = new();

    public Action? UserProfileRequested { get; set; }

    public Action? BidHistoryRequested { get; set; }
    public int SelectedAuctionId { get; set; }

    public Action<Auction_Core.Models.Auction>? SellerOfAuctionRequested { get; set; }
    public Action<Auction_Core.Models.Auction>? BuyerOfAuctionRequested { get; set; }

    private AuctionRepository _auctionRepo;
    private SessionService _sessionService;

    public HomeViewModel(AuctionRepository auctionRepo, SessionService sessionService)
    {
        _auctionRepo = auctionRepo;
        _sessionService = sessionService;
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

            if (auction.User.ID == _sessionService.CurrentUser?.ID) //TODO: use the current user's ID instead of 1
            {
                UserAuctions.Add(auction);
            }
        }
    }



    public void GoToSellerOfAuction()
    {
        var selectedAuction = UserAuctions.FirstOrDefault(a => a.Id == SelectedAuctionId);
        if (selectedAuction is not null)
        {
            SellerOfAuctionRequested?.Invoke(selectedAuction);
        }
    }

    public void GoToBuyerOfAuction()
    {
        var selectedAuction = Auctions.FirstOrDefault(a => a.Id == SelectedAuctionId);
        if (selectedAuction is not null)
        {
            BuyerOfAuctionRequested?.Invoke(selectedAuction);
        }
    }
}