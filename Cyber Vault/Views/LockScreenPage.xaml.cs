using Cyber_Vault.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Cyber_Vault.Services;
using Cyber_Vault.Helpers;
using Cyber_Vault.DB;
using Cyber_Vault.DL;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;

namespace Cyber_Vault.Views;

public sealed partial class LockScreenPage : Page
{

    public LockScreenViewModel ViewModel
    {
        get;
    }

    public LockScreenPage()
    {
        ViewModel = App.GetService<LockScreenViewModel>();
        InitializeComponent();

        App.MainWindow.ExtendsContentIntoTitleBar = true;
        App.MainWindow.SetTitleBar(AppTitleBar);
        App.AppTitlebar = AppTitleBarText;
        AppTitleBarText.Text = "AppDisplayName".GetLocalized();

        _ = ActivationService.StartupAsync();

        Login_Username_TextBox.Text = "umer123";
        Login_PasswordBox.Password = "Umerfarooq*123";

        if (CredentialsManager.CheckInDatabase())
        {
            Login_StackPanel.Visibility = Visibility.Visible;
            if(CredentialsManager.GetUsernameFromMemory() != null) {
                var ptr = IntPtr.Zero;
                try
                {
                    ptr = Marshal.SecureStringToGlobalAllocUnicode(CredentialsManager.GetUsernameFromMemory()!);
                    Login_Username_TextBox.Text = Marshal.PtrToStringUni(ptr);
                    Login_PasswordBox.Focus(FocusState.Keyboard);
                }
                finally
                {
                    Marshal.ZeroFreeGlobalAllocUnicode(ptr);
                }
                
            }
        }
        else
        {
            Signup_StackPanel.Visibility = Visibility.Visible;
        }

        
    }

    private async void Login_Button_Click(object _, RoutedEventArgs e)
    {
        if (Login_Username_TextBox.Text.Length < 5)
        {
            Login_Username_TextBox.Text = string.Empty;
            Login_PasswordBox.Password = string.Empty;
            // TODO: Show error message
            return;
        }

        if (Login_PasswordBox.Password.Length < 8)
        {
            Login_Username_TextBox.Text = string.Empty;
            Login_PasswordBox.Password = string.Empty;
            // TODO: Show error message
            return;
        }


        if (!CredentialsManager.MatchInDatabase(Login_Username_TextBox.Text, Login_PasswordBox.Password))
        {
            Login_Username_TextBox.Text = string.Empty;
            Login_PasswordBox.Password = string.Empty;
            // TODO: Show error message
            return;
        }

        CredentialsManager.StoreUsernameInMemory(Login_Username_TextBox.Text);
        CredentialsManager.StorePasswordInMemory(Login_PasswordBox.Password);
        AccountDL.LoadAccountsFromDatabase();
        BackupCodeDL.LoadBackupCodesFromDatabase();
        CreditCardDL.LoadCreditCardsFromDatabase();

        UIElement? _shell = App.GetService<ShellPage>();
        App.MainWindow.Content = _shell ?? new Frame();
        
        await ActivationService.StartupAsync();
    }

    private void Signup_Button_Click(object sender, RoutedEventArgs e)
    {
        if (Signup_Username_TextBox.Text.Length < 5)
        {
            Signup_Username_TextBox.Text = string.Empty;
            Signup_PasswordBox.Password = string.Empty;
            // TODO: Show error message
            return;
        }

        if (Signup_PasswordBox.Password.Length < 8)
        {
            Signup_Username_TextBox.Text = string.Empty;
            Signup_PasswordBox.Password = string.Empty;
            // TODO: Show error message
            return;
        }

        CredentialsManager.StoreInDatabase(Signup_Username_TextBox.Text, Signup_PasswordBox.Password);

        // TODO: Show success message

        Signup_StackPanel.Visibility = Visibility.Collapsed;

        Login_StackPanel.Visibility = Visibility.Visible;

    }

    private void UsernameInfo_Button_Click(object sender, RoutedEventArgs e)
    {
        Username_TeachingTip.IsOpen = true;
    }

    private void PasswordInfo_Button_Click(object sender, RoutedEventArgs e)
    {
        Password_TeachingTip.IsOpen = true;
    }
}
