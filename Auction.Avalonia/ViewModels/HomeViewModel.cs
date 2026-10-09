namespace Auction.Avalonia.ViewModels;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Auction_Avalonia.Services;
using CommunityToolkit.Mvvm.Input;
using System;
using Auction_Core.Models;
using Auction_Core.Repository;

public partial class HomeViewModel : ViewModelBase
{
    public ObservableCollection<Auction> UserAuctions { get; private set; } = new();
    public ObservableCollection<Auction> Auctions { get; private set; } = new();

    private AuctionRepository _auctionRepo;
    private SessionService _sessionService;

    public HomeViewModel(AuctionRepository auctionRepo, SessionService sessionService)
    {
        _auctionRepo = auctionRepo;
        _sessionService = sessionService;
    }
    internal Action? GoToSetForSale { get; set; }

    [RelayCommand]
    private void SetForSale()
    {
        GoToSetForSale?.Invoke();
    }
    public async void RefreshAuctionsAsync()
    {
        Auctions.Clear();
        UserAuctions.Clear();
        var allAuctions = await _auctionRepo.GetAllAuctionsAsync();
        
        foreach (var auction in allAuctions)
        {
            Auctions.Add(auction);

            if (auction.User.ID == _sessionService.CurrentUser?.ID) //TODO: use the current user's ID instead of 1
            {
                UserAuctions.Add(auction);
            }
        }
    }


}