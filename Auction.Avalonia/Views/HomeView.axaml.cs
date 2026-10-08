using Auction.Avalonia.ViewModels;
using Avalonia;
using Auction_Core.Models;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Auction.Avalonia.Views;

public partial class HomeView : UserControl
{
    public HomeView()
    {
        InitializeComponent();
        this.AttachedToVisualTree += (_, _) =>
        {
            if (DataContext is HomeViewModel viewModel)
            {
                viewModel.RefreshAuctionsAsync();
            }
        };
    }

    private void TableView_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count <= 0) return;
        object selectedItem = e.AddedItems[0];
        if (selectedItem == null) return;
        Auction_Core.Models.Auction auction = selectedItem as Auction_Core.Models.Auction;
        int auctionId = auction.Id;

        TableView tableView = sender as TableView;

        tableView?.UnselectAll();


        if (DataContext is HomeViewModel viewModel)
        {
            viewModel.SelectedAuctionId = auctionId;

            if (tableView.Name == "yourAuctionsTable")
            {
                viewModel.GoToSellerOfAuction();
            }
        }
    }
}