using Avalonia.Controls;
using Avalonia.Input;
using Auction_Avalonia.ViewModels;

namespace Auction_Avalonia.Views;

public partial class SetForSaleView : UserControl
{
    public SetForSaleView()
    {
        InitializeComponent();
    }


    private void OnBackRequested(object? sender, PointerPressedEventArgs e)
    {
        (DataContext as SetForSaleViewModel)?.BackRequested?.Invoke();
    }    
}