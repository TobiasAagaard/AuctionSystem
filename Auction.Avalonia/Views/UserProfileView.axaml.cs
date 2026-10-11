using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Auction_Avalonia.ViewModels;

namespace Auction_Avalonia.Views;

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