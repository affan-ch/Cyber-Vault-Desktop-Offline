using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;

namespace Cyber_Vault.Utils;

internal class MessageDialogHelper
{
    public static async void ShowMessageDialog(XamlRoot xamlRoot, string title, string text)
    {
        var dialog = new ContentDialog
        {
            XamlRoot = xamlRoot,
            Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
            Title = title,
            PrimaryButtonText = "Close",
            DefaultButton = ContentDialogButton.Primary,
            Content = new TextBlock
            {
                Text = text
            }
        };

        await dialog.ShowAsync();
    }
}
