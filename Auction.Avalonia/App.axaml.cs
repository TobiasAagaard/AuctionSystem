using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Auction_Avalonia.Services;
using Auction_Avalonia.ViewModels;
using Auction_Avalonia.Views;
using Microsoft.Extensions.DependencyInjection;
using Auction_Core;
using Auction_Core.Repository;
using System;

namespace Auction_Avalonia;

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
        services.AddTransient<LoginViewModel>();
        services.AddTransient<CreateUserViewModel>();
        services.AddTransient<HomeViewModel>();
        services.AddSingleton<ToastService>();
        services.AddSingleton<SessionService>();
        services.AddTransient<AuctionRepository>();
        services.AddTransient<UserProfileViewModel>();

        var serviceProvider = services.BuildServiceProvider();
        Services = serviceProvider;

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
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