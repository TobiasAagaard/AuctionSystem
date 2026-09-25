
using CommunityToolkit.Mvvm.ComponentModel;

namespace Auction.Avalonia.ViewModels;

public partial class ToastViewModel : ObservableObject
{
    public string Message { get; set; } = string.Empty;

    public NotificationType Type { get; init; }

    public ToastViewModel(string message, NotificationType type)
    {
        this.Message = message;
        this.Type = type;
    }

    // Accent used for the side stripe/dot only; the card itself stays a translucent neutral color.
    public string AccentBrush => Type switch
    {
        NotificationType.Info => "#3B82F6",
        NotificationType.Success => "#22C55E",
        NotificationType.Warning => "#F59E0B",
        NotificationType.Error => "#EF4444",
        _ => "#9CA3AF"
    };

    public enum NotificationType
    {
        Info,
        Success,
        Warning,
        Error,
    }
    
}