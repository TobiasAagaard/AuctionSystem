using Auction.Avalonia.ViewModels;
using Avalonia.Controls;
using Avalonia.Input;

namespace Auction.Avalonia.Views;

public partial class LoginView : UserControl
{
    public LoginView()
    {
        InitializeComponent();
    }

    private void OnCreateUserPressed(object? sender, PointerPressedEventArgs e)
    {
        (DataContext as LoginViewModel)?.GoToCreateUserCommand.Execute(null);
    }
}
