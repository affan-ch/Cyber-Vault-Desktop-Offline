using Cyber_Vault.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Text;
using Cyber_Vault.Utils;
using Cyber_Vault.BL;
using Cyber_Vault.DL;
using Cyber_Vault.DB;
using Microsoft.UI.Xaml.Input;

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

    private void AddSecureNote_Button_Click(object sender, RoutedEventArgs e)
    {
        ErrorContainer_Grid.Visibility = Visibility.Collapsed;
        AddSecureNote_Grid.Visibility = Visibility.Visible;
    }

    private void SearchBar_KeyUp(object sender, KeyRoutedEventArgs e)
    {

    }


    private void TitleInfo_Button_Click(object sender, RoutedEventArgs e)
    {
        Category_TeachingTip.IsOpen = false;
        TagsInfo_TeachingTip.IsOpen = false;
        CustomCategory_TeachingTip.IsOpen = false;
        Notes_TeachingTip.IsOpen = false;
        Title_TeachingTip.IsOpen = true;
    }

    private void CategoryInfo_Button_Click(object sender, RoutedEventArgs e)
    {
        Title_TeachingTip.IsOpen = false;
        TagsInfo_TeachingTip.IsOpen = false;
        CustomCategory_TeachingTip.IsOpen = false;
        Notes_TeachingTip.IsOpen = false;
        Category_TeachingTip.IsOpen = true;
    }

    private void CustomCategoryInfo_Button_Click(object sender, RoutedEventArgs e)
    {
        Title_TeachingTip.IsOpen = false;
        Category_TeachingTip.IsOpen = false;
        TagsInfo_TeachingTip.IsOpen = false;
        Notes_TeachingTip.IsOpen = false;
        CustomCategory_TeachingTip.IsOpen = true;
    }

    private void TagsInfo_Button_Click(object sender, RoutedEventArgs e)
    {
        Title_TeachingTip.IsOpen = false;
        Category_TeachingTip.IsOpen = false;
        CustomCategory_TeachingTip.IsOpen = false;
        Notes_TeachingTip.IsOpen = false;
        TagsInfo_TeachingTip.IsOpen = true;
    }

    private void NotesInfo_Button_Click(object sender, RoutedEventArgs e)
    {
        Title_TeachingTip.IsOpen = false;
        Category_TeachingTip.IsOpen = false;
        CustomCategory_TeachingTip.IsOpen = false;
        TagsInfo_TeachingTip.IsOpen = false;
        Notes_TeachingTip.IsOpen = true;
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

        var documentRange = editor.Document.GetRange(0, TextConstants.MaxUnitCount);
        var background = (SolidColorBrush)App.Current.Resources["TextControlBackgroundFocused"];

        if (background != null)
        {
            documentRange.CharacterFormat.BackgroundColor = background.Color;
        }
    }

    private void Save_Button_Click(object sender, RoutedEventArgs e)
    {
        var note = GetInputObject();
        if (note != null)
        {
            SecureNoteDL.AddSecureNote(note);
            SecureNoteDB.StoreSecureNotes(note);
            ClearFields();
            MessageDialogHelper.ShowMessageDialog(XamlRoot, "Success", "Secure Note added successfully");
        }

    }

    private SecureNote? GetInputObject()
    {
        if (Category_ComboBox.SelectedIndex == -1)
        {
            MessageDialogHelper.ShowMessageDialog(XamlRoot, "Error", "Category is required");
            return null;
        }

        var Title = Title_TextBox.Text;
        var Category = Category_ComboBox.SelectedValue.ToString();
        var CustomCategory = CustomCategory_TextBox.Text;
        var Tag1 = Tag1_TextBox.Text;
        var Tag2 = Tag2_TextBox.Text;
        var Tag3 = Tag3_TextBox.Text;
        var Tag4 = Tag4_TextBox.Text;
        editor.Document.GetText(TextGetOptions.UseCrlf, out var RichEditBoxContent);

        if (string.IsNullOrEmpty(Title))
        {
            MessageDialogHelper.ShowMessageDialog(XamlRoot, "Error", "Title is required");
            return null;
        }

        if (string.IsNullOrEmpty(Category))
        {
            MessageDialogHelper.ShowMessageDialog(XamlRoot, "Error", "Category is required");
            return null;
        }

        if (Category == "Custom" && string.IsNullOrEmpty(CustomCategory))
        {
            MessageDialogHelper.ShowMessageDialog(XamlRoot, "Error", "Custom Category is required");
            return null;
        }

        if (Title.Length > 15)
        {
            MessageDialogHelper.ShowMessageDialog(XamlRoot, "Error", "Title should be less than 15 characters");
            return null;
        }

        if (Title.Length < 3)
        {
            MessageDialogHelper.ShowMessageDialog(XamlRoot, "Error", "Title should be more than 3 characters");
            return null;
        }

        if (Category == "Custom" && CustomCategory.Length > 15)
        {
            MessageDialogHelper.ShowMessageDialog(XamlRoot, "Error", "Custom Category should be less than 15 characters");
            return null;
        }

        if (Category == "Custom" && CustomCategory.Length < 3)
        {
            MessageDialogHelper.ShowMessageDialog(XamlRoot, "Error", "Custom Category should be more than 3 characters");
            return null;
        }

        if (Tag1.Length > 10)
        {
            MessageDialogHelper.ShowMessageDialog(XamlRoot, "Error", "Tag1 should be less than 10 characters");
            return null;
        }

        if (Tag2.Length > 10)
        {
            MessageDialogHelper.ShowMessageDialog(XamlRoot, "Error", "Tag2 should be less than 10 characters");
            return null;
        }

        if (Tag3.Length > 10)
        {
            MessageDialogHelper.ShowMessageDialog(XamlRoot, "Error", "Tag3 should be less than 10 characters");
            return null;
        }

        if (Tag4.Length > 10)
        {
            MessageDialogHelper.ShowMessageDialog(XamlRoot, "Error", "Tag4 should be less than 10 characters");
            return null;
        }

        if (Tag1.Length < 3 && !string.IsNullOrEmpty(Tag1))
        {
            MessageDialogHelper.ShowMessageDialog(XamlRoot, "Error", "Tag1 should be more than 3 characters");
            return null;
        }

        if (Tag2.Length < 3 && !string.IsNullOrEmpty(Tag2))
        {
            MessageDialogHelper.ShowMessageDialog(XamlRoot, "Error", "Tag2 should be more than 3 characters");
            return null;
        }

        if (Tag3.Length < 3 && !string.IsNullOrEmpty(Tag3))
        {
            MessageDialogHelper.ShowMessageDialog(XamlRoot, "Error", "Tag3 should be more than 3 characters");
            return null;
        }

        if (Tag4.Length < 3 && !string.IsNullOrEmpty(Tag4))
        {
            MessageDialogHelper.ShowMessageDialog(XamlRoot, "Error", "Tag4 should be more than 3 characters");
            return null;
        }

        var Note = new SecureNote
        {
            Title = Title,
            Category = (Category == "Custom") ? CustomCategory : Category,
            Tag1 = Tag1,
            Tag2 = Tag2,
            Tag3 = Tag3,
            Tag4 = Tag4,
            Note = RichEditBoxContent,
            DateAdded = DateTime.Now.ToString(),
            DateModified = DateTime.Now.ToString()
        };

        return Note;
    }

    private void ClearFields()
    {
        Title_TextBox.Text = string.Empty;
        Category_ComboBox.SelectedIndex = -1;
        CustomCategory_TextBox.Text = string.Empty;
        Tag1_TextBox.Text = string.Empty;
        Tag2_TextBox.Text = string.Empty;
        Tag3_TextBox.Text = string.Empty;
        Tag4_TextBox.Text = string.Empty;
        editor.Document.SetText(TextSetOptions.None, string.Empty);
    }
}
