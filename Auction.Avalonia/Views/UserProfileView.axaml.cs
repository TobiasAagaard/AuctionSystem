using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Auction.Avalonia.ViewModels;

namespace Auction.Avalonia.Views;

public partial class UserProfileView : UserControl
{
    public UserProfileView()
    {
        InitializeComponent();
        this.AttachedToVisualTree += (_, _) =>
        {
            if (DataContext is UserProfileViewModel viewModel)
            {
                viewModel.RefreshUserProfile();
            }
        };
    }
}