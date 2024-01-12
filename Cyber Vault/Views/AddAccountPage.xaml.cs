using Cyber_Vault.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace Cyber_Vault.Views;

public sealed partial class AddAccountPage : Page
{
    public AddAccountViewModel ViewModel
    {
        get;
    }

    public AddAccountPage()
    {
        ViewModel = App.GetService<AddAccountViewModel>();
        InitializeComponent();
    }
}
