using Cyber_Vault.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace Cyber_Vault.Views;

public sealed partial class CreditCardsPage : Page
{
    public CreditCardsViewModel ViewModel
    {
        get;
    }

    public CreditCardsPage()
    {
        ViewModel = App.GetService<CreditCardsViewModel>();
        InitializeComponent();
    }
}
