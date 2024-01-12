using Cyber_Vault.Contracts.Services;
using Cyber_Vault.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Cyber_Vault.Services;
using Cyber_Vault.Helpers;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Media;

namespace Cyber_Vault.Views;

public sealed partial class HomePage : Page
{

    public HomeViewModel ViewModel
    {
        get;
    }

    public HomePage()
    {
        ViewModel = App.GetService<HomeViewModel>();
        InitializeComponent();

        _ = ActivationService.StartupAsync();
    }

    private async void Button_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        UIElement? _shell = App.GetService<ShellPage>();
        App.MainWindow.Content = _shell ?? new Frame();
        
        await ActivationService.StartupAsync();

    }



}
