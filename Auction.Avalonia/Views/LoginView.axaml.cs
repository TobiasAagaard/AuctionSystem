using Auction_Avalonia.ViewModels;
using Avalonia.Controls;
using Avalonia.Input;

namespace Auction_Avalonia.Views;

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
