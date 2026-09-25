using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using Auction.Avalonia.ViewModels;
using System.Threading.Tasks;
using Avalonia.Threading;

namespace Auction.Avalonia.Services
{
    public partial class ToastService : ObservableObject
    {
        public ObservableCollection<ToastViewModel> Notifications { get; } = new();

        public void Show(string message, ToastViewModel.NotificationType type = ToastViewModel.NotificationType.Info, int duration = 3000)
        {
            var notification = new ToastViewModel(message, type);
            Notifications.Add(notification);

            if (duration > 0)
            {
                _ = Task.Delay(duration).ContinueWith(_ => Dispatcher.UIThread.Post(() => Notifications.Remove(notification)));
            }
        }
        [RelayCommand]
        private void Dismiss(ToastViewModel notification)
        {
            Notifications.Remove(notification);
        }
    }
}