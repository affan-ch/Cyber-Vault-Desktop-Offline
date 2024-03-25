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
    private readonly RadioButtons radioButtons = new()
    {
        Visibility = Visibility.Collapsed
    };
    private int currentSecureNoteId = 0;

    public SecureNotesViewModel ViewModel
    {
        get;
    }

    public SecureNotesPage()
    {
        ViewModel = App.GetService<SecureNotesViewModel>();
        InitializeComponent();
        RefreshSecureNotesListView();
    }

    // Load Page: Add Secure Note
    private void AddSecureNote_Button_Click(object sender, RoutedEventArgs e)
    {
        foreach (var rb in radioButtons.Items.Cast<RadioButton>())
        {
            rb.IsChecked = false;
        }
        ClearFields();
        ErrorContainer_Grid.Visibility = Visibility.Collapsed;
        AddSecureNote_Grid.Visibility = Visibility.Visible;
    }

    // Search Bar Key Up
    private void SearchBar_KeyUp(object sender, KeyRoutedEventArgs e)
    {

    }

    // Title Info Button Click
    private void TitleInfo_Button_Click(object sender, RoutedEventArgs e)
    {
        Category_TeachingTip.IsOpen = false;
        TagsInfo_TeachingTip.IsOpen = false;
        CustomCategory_TeachingTip.IsOpen = false;
        Notes_TeachingTip.IsOpen = false;
        Title_TeachingTip.IsOpen = true;
    }

    // Category Info Button Click
    private void CategoryInfo_Button_Click(object sender, RoutedEventArgs e)
    {
        Title_TeachingTip.IsOpen = false;
        TagsInfo_TeachingTip.IsOpen = false;
        CustomCategory_TeachingTip.IsOpen = false;
        Notes_TeachingTip.IsOpen = false;
        Category_TeachingTip.IsOpen = true;
    }

    // Custom Category Info Button Click
    private void CustomCategoryInfo_Button_Click(object sender, RoutedEventArgs e)
    {
        Title_TeachingTip.IsOpen = false;
        Category_TeachingTip.IsOpen = false;
        TagsInfo_TeachingTip.IsOpen = false;
        Notes_TeachingTip.IsOpen = false;
        CustomCategory_TeachingTip.IsOpen = true;
    }

    // Tags Info Button Click
    private void TagsInfo_Button_Click(object sender, RoutedEventArgs e)
    {
        Title_TeachingTip.IsOpen = false;
        Category_TeachingTip.IsOpen = false;
        CustomCategory_TeachingTip.IsOpen = false;
        Notes_TeachingTip.IsOpen = false;
        TagsInfo_TeachingTip.IsOpen = true;
    }

    // Notes Info Button Click
    private void NotesInfo_Button_Click(object sender, RoutedEventArgs e)
    {
        Title_TeachingTip.IsOpen = false;
        Category_TeachingTip.IsOpen = false;
        CustomCategory_TeachingTip.IsOpen = false;
        TagsInfo_TeachingTip.IsOpen = false;
        Notes_TeachingTip.IsOpen = true;
    }

    // Category ComboBox Selection Changed
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

    // RichEditBox Focus
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

    // Save Secure Note
    private void Save_Button_Click(object sender, RoutedEventArgs e)
    {
        var note = GetInputObject();
        if (note != null)
        {
            SecureNoteDL.AddSecureNote(note);
            SecureNoteDB.StoreSecureNotes(note);
            ClearFields();
            RefreshSecureNotesListView();
            MessageDialogHelper.ShowMessageDialog(XamlRoot, "Success", "Secure Note added successfully");
        }
    }

    // Get Secure Note Input Object
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
            Id = SecureNoteDB.GetMaxId() + 1,
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

    // Clear Fields
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

    // Add Secure Note Tile in ListView (Left Sidebar)
    private void AddSecureNoteInListView(int id)
    {
        NoSecureNotes_Grid.Visibility = Visibility.Collapsed;
        SecureNotes_ScrollViewer.Visibility = Visibility.Visible;
        NoSecureNotes_Grid.Visibility = Visibility.Collapsed;

        // Create a new StackPanel dynamically
        var noteContainer = new Grid
        {
            Margin = new Thickness(0, 1, 20, 5),
            Height = 70,
            Background = (Brush)Application.Current.Resources["DesktopAcrylicTransparentBrush"],
            CornerRadius = new CornerRadius(5),
        };

        // Create ColumnDefinitions for the Grid
        noteContainer.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto }); // For Secure Note Icon
        noteContainer.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) }); // For Title and Category
        noteContainer.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto }); // For Space
        noteContainer.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto }); // For Right icon


        noteContainer.PointerEntered += (sender, e) =>
        {
            noteContainer.Background = (Brush)Application.Current.Resources["LayerOnAcrylicFillColorDefaultBrush"];
        };

        noteContainer.PointerExited += (sender, e) =>
        {
            noteContainer.Background = (Brush)Application.Current.Resources["DesktopAcrylicTransparentBrush"];
        };

        var radioButton = new RadioButton
        {
            Name = id.ToString(),
            IsChecked = false,
            Visibility = Visibility.Collapsed,
        };

        radioButton.Unchecked += (sender, e) =>
        {
            noteContainer.Background = (Brush)Application.Current.Resources["DesktopAcrylicTransparentBrush"];

            noteContainer.PointerEntered += (sender, e) =>
            {
                noteContainer.Background = (Brush)Application.Current.Resources["LayerOnAcrylicFillColorDefaultBrush"];
            };

            noteContainer.PointerExited += (sender, e) =>
            {
                noteContainer.Background = (Brush)Application.Current.Resources["DesktopAcrylicTransparentBrush"];
            };
        };

        radioButtons.Items.Add(radioButton);


        var note = SecureNoteDL.GetSecureNoteById(id);

        // Create the StackPanel for Title and Category
        var innerStackPanel = new StackPanel
        {
            Orientation = Orientation.Vertical,
            Margin = new Thickness(5, 14, 0, 0),
        };

        // Title TextBlock
        var title = new TextBlock
        {
            Text = note!.Title ?? "Personal Notes",
            Style = (Style)Application.Current.Resources["BaseTextBlockStyle"],
            FontSize = 16
        };

        // Category TextBlock
        var category = new TextBlock
        {
            Text = note.Category ?? "Custom",
            Style = (Style)Application.Current.Resources["BaseTextBlockStyle"],
            FontSize = 12,
            Opacity = 0.9
        };

        // Add TextBlocks to StackPanel
        innerStackPanel.Children.Add(title);
        innerStackPanel.Children.Add(category);
        innerStackPanel.Children.Add(radioButton);

        // Create Secure Note Icon
        var noteIcon = new FontIcon
        {
            Glyph = "\uE70B",
            Opacity = 0.8,
            Margin = new Thickness(15, 10, 10, 10),
            FontSize = 27,
            VerticalAlignment = VerticalAlignment.Center
        };

        // Create Right FontIcon
        var rightIcon = new FontIcon
        {
            Glyph = "\xE974",
            Opacity = 0.8,
            Margin = new Thickness(0, 0, 5, 0),
        };

        // Set Grid.Column for each element
        Grid.SetColumn(noteIcon, 0);
        Grid.SetColumn(innerStackPanel, 1);
        Grid.SetColumn(rightIcon, 3);

        // Add the Image, inner StackPanel, and FontIcon to the dynamic StackPanel
        noteContainer.Children.Add(noteIcon);
        noteContainer.Children.Add(innerStackPanel);
        noteContainer.Children.Add(rightIcon);

        noteContainer.PointerPressed += (sender, e) =>
        {
            foreach (var rb in radioButtons.Items.Cast<RadioButton>())
            {
                if (rb.Name == id.ToString())
                {
                    rb.IsChecked = true;
                    currentSecureNoteId = id;

                    noteContainer.Background = (Brush)Application.Current.Resources["LayerOnAcrylicFillColorDefaultBrush"];
                    noteContainer.PointerEntered += (sender, e) =>
                    {
                        noteContainer.Background = (Brush)Application.Current.Resources["LayerOnAcrylicFillColorDefaultBrush"];
                    };
                    noteContainer.PointerExited += (sender, e) =>
                    {
                        noteContainer.Background = (Brush)Application.Current.Resources["LayerOnAcrylicFillColorDefaultBrush"];
                    };

                    ErrorContainer_Grid.Visibility = Visibility.Collapsed;
                    AddSecureNote_Grid.Visibility = Visibility.Collapsed;
                    //ViewAccount_Grid.Visibility = Visibility.Visible;
                    //RenderUserInterface(account);
                }
                else
                {
                    rb.IsChecked = false;
                }
            }
        };

        SecureNotes_ListView.Children.Add(noteContainer);
    }

    // Refresh Secure Notes List View
    public void RefreshSecureNotesListView()
    {
        SecureNotes_ListView.Children.Clear();
        radioButtons.Items.Clear();

        var notes = SecureNoteDL.GetSecureNotes();

        if (notes.Count == 0)
        {
            SecureNotes_ScrollViewer.Visibility = Visibility.Collapsed;
            NoSecureNotes_Grid.Visibility = Visibility.Visible;
        }
        else
        {
            SecureNotes_ScrollViewer.Visibility = Visibility.Visible;
            NoSecureNotes_Grid.Visibility = Visibility.Collapsed;

            foreach (var note in notes)
            {
                AddSecureNoteInListView(note.Id ?? 0);
            }
        }
    }

}
