using Cyber_Vault.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace Cyber_Vault.Views;

public sealed partial class DocumentsPage : Page
{
    public DocumentsViewModel ViewModel
    {
        get;
    }

    public DocumentsPage()
    {
        ViewModel = App.GetService<DocumentsViewModel>();
        InitializeComponent();
    }
}
