using Cyber_Vault.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Windows.Storage;
using Microsoft.UI.Text;
using Windows.UI;

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

    private void AddSecureNote_Button_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        ErrorContainer_Grid.Visibility = Visibility.Collapsed;
        AddSecureNote_Grid.Visibility = Visibility.Visible;
    }

    private void SearchBar_KeyUp(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
    {

    }


    private void TitleInfo_Button_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        Category_TeachingTip.IsOpen = false;
        TagsInfo_TeachingTip.IsOpen = false;
        CustomCategory_TeachingTip.IsOpen = false;
        Title_TeachingTip.IsOpen = true;
    }

    private void CategoryInfo_Button_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        Title_TeachingTip.IsOpen = false;
        TagsInfo_TeachingTip.IsOpen = false;
        CustomCategory_TeachingTip.IsOpen = false;
        Category_TeachingTip.IsOpen = true;
    }

    private void CustomCategory_Button_Click(object sender, RoutedEventArgs e)
    {
        Title_TeachingTip.IsOpen = false;
        Category_TeachingTip.IsOpen = false;
        TagsInfo_TeachingTip.IsOpen = false;
        CustomCategory_TeachingTip.IsOpen = true;
    }

    private void TagsInfo_Button_Click(object sender, RoutedEventArgs e)
    {
        Title_TeachingTip.IsOpen = false;
        Category_TeachingTip.IsOpen = false;
        CustomCategory_TeachingTip.IsOpen = false;
        TagsInfo_TeachingTip.IsOpen = true;
    }

    private void Category_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (CustomCategory_Container != null)
        {
            if (Category_ComboBox.SelectedIndex >= 0)
            {
                if (Category_ComboBox.SelectedValue.ToString() == "Custom")
                {
                    CustomCategory_Container.Visibility = Visibility.Visible;
                }
                else
                {
                    CustomCategory_Container.Visibility = Visibility.Collapsed;
                }
            }
        }

    }


    private void Editor_GotFocus(object sender, RoutedEventArgs e)
    {
        editor.Document.GetText(TextGetOptions.UseCrlf, out _);

        // reset colors to correct defaults for Focused state
        var documentRange = editor.Document.GetRange(0, TextConstants.MaxUnitCount);
        var background = (SolidColorBrush)App.Current.Resources["TextControlBackgroundFocused"];

        if (background != null)
        {
            documentRange.CharacterFormat.BackgroundColor = background.Color;
        }
    }

    private void Save_Button_Click(object sender, RoutedEventArgs e)
    {

    }



}
