using CommunityToolkit.Mvvm.ComponentModel;

namespace Auction.Avalonia.ViewModels;

/// <summary>
/// The shell view model. It holds whichever page is currently shown in MainWindow
/// and handles switching between pages.
/// </summary>
public partial class MainViewModel : ViewModelBase
{
    private readonly LoginViewModel _loginViewModel;
    private readonly CreateUserViewModel _createUserViewModel;

    [ObservableProperty]
    public partial ViewModelBase CurrentPage { get; set; }

    public MainViewModel(LoginViewModel loginViewModel, CreateUserViewModel createUserViewModel)
    {
        _loginViewModel = loginViewModel;
        _createUserViewModel = createUserViewModel;

        _loginViewModel.CreateUserRequested = ShowCreateUser;
        _createUserViewModel.BackRequested = ShowLogin;

        CurrentPage = _loginViewModel;
    }

    private void ShowLogin() => CurrentPage = _loginViewModel;

    private void ShowCreateUser() => CurrentPage = _createUserViewModel;
}
