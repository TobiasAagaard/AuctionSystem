using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Auction.Avalonia.ViewModels;
using Auction.Avalonia.Views;
using Microsoft.Extensions.DependencyInjection;
using Auction_Core;
using System;

namespace Auction.Avalonia;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; } = null!;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var services = new ServiceCollection();

        services.AddCore();

        services.AddTransient<MainViewModel>();

        var serviceProvider = services.BuildServiceProvider();
        Services = serviceProvider;

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Dispose the service provider when the application exits
            desktop.Exit += (_, _) => serviceProvider.Dispose();
            var mainViewModel = serviceProvider.GetRequiredService<MainViewModel>();

            desktop.MainWindow = new MainWindow
            {
                DataContext = mainViewModel,
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}