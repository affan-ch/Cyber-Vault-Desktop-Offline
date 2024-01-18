using Cyber_Vault.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Cyber_Vault.Services;
using Cyber_Vault.Helpers;
using Cyber_Vault.DB;

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

        App.MainWindow.ExtendsContentIntoTitleBar = true;
        App.MainWindow.SetTitleBar(AppTitleBar);
        App.AppTitlebar = AppTitleBarText;
        AppTitleBarText.Text = "AppDisplayName".GetLocalized();

        _ = ActivationService.StartupAsync();


        if(MasterKey.IsInDatabase())
        {
            Login_StackPanel.Visibility = Visibility.Visible;
        }
        else
        {
            Signup_StackPanel.Visibility = Visibility.Visible;
        }
    }

    private async void Login_Button_Click(object _, RoutedEventArgs e)
    {
        var key = Login_PasswordBox.Password;

        if(key.Length < 8)
        {
            Login_PasswordBox.Password = string.Empty;
            // TODO: Show error message
            return;
        }

        if(!MasterKey.IsCorrect(key))
        {
            Login_PasswordBox.Password = string.Empty;
            // TODO: Show error message
            return;
        }

        MasterKey.StoreInMemory(key);


        UIElement? _shell = App.GetService<ShellPage>();
        App.MainWindow.Content = _shell ?? new Frame();
        
        await ActivationService.StartupAsync();
    }

    private void Signup_Button_Click(object sender, RoutedEventArgs e)
    {
        var key = Signup_PasswordBox.Password;

        if(key.Length < 8)
        {
            Signup_PasswordBox.Password = string.Empty;
            // TODO: Show error message
            return;
        }

        MasterKey.StoreInDatabase(key);

        // TODO: Show success message

        Signup_StackPanel.Visibility = Visibility.Collapsed;

        Login_StackPanel.Visibility = Visibility.Visible;

    }
}
