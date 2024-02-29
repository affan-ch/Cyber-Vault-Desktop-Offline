using Cyber_Vault.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace Cyber_Vault.Views;

public sealed partial class SecureNotesPage : Page
{
    public SecureNotesViewModel ViewModel
    {
        get;
    }

    public SecureNotesPage()
    {
        ViewModel = App.GetService<SecureNotesViewModel>();
        InitializeComponent();
    }
}
