using Cyber_Vault.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace Cyber_Vault.Views;

public sealed partial class AccountsPage : Page
{
    public AccountsViewModel ViewModel
    {
        get;
    }

    public AccountsPage()
    {
        ViewModel = App.GetService<AccountsViewModel>();
        InitializeComponent();
    }
}
