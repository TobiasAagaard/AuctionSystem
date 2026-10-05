using Auction.Avalonia.ViewModels;
using Avalonia;
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
}