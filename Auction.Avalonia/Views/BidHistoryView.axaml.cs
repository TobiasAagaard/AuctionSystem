using Auction_Avalonia.ViewModels;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Auction_Avalonia.Views;

public partial class BidHistoryView : UserControl
{
    public BidHistoryView()
    {
        InitializeComponent();
        this.AttachedToVisualTree += (_, _) =>
        {
            if (DataContext is BidHistoryViewModel viewModel)
            {
                viewModel.RefreshBidHistoryAsync();
            }
        };
    }
}