using Auction.Avalonia.ViewModels;
using Avalonia.Controls;
using Avalonia.Input;

namespace Auction.Avalonia.Views;

public partial class CreateUserView : UserControl
{
    public CreateUserView()
    {
        InitializeComponent();
    }

    private void OnCancelPressed(object? sender, PointerPressedEventArgs e)
    {
        (DataContext as CreateUserViewModel)?.GoBackToLoginCommand.Execute(null);
    }
}
