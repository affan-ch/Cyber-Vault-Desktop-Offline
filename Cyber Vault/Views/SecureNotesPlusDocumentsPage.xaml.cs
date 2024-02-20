using Cyber_Vault.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace Cyber_Vault.Views;

public sealed partial class SecureNotesPlusDocumentsPage : Page
{
    public SecureNotesPlusDocumentsViewModel ViewModel
    {
        get;
    }

    public SecureNotesPlusDocumentsPage()
    {
        ViewModel = App.GetService<SecureNotesPlusDocumentsViewModel>();
        InitializeComponent();
    }
}
