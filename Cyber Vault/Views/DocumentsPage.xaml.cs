using Cyber_Vault.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Cyber_Vault.BL;
using Cyber_Vault.DL;
using System.Diagnostics;
using Windows.Storage.Pickers;
using Cyber_Vault.DB;
using Cyber_Vault.Utils;

namespace Cyber_Vault.Views;

public sealed partial class DocumentsPage : Page
{
    //private int currentDocumentId = 0;
    //private int _selectedDocumentIndex = -1;
    private int filesCount = 1;

    private readonly RadioButtons radioButtons = new()
    {
        Visibility = Visibility.Collapsed
    };
    public DocumentsViewModel ViewModel
    {
        get;
    }

    public DocumentsPage()
    {
        ViewModel = App.GetService<DocumentsViewModel>();
        InitializeComponent();
        RefreshDocumentsListView();
    }
    // Add Document Tile in ListView (Left Sidebar)
    private void AddDocumentInListView(int? id, string? title, string? subtitle)
    {
        NoDocuments_Grid.Visibility = Visibility.Collapsed;

        // Create a new StackPanel dynamically
        var documentContainer = new Grid
        {
            Margin = new Thickness(0, 1, 20, 5),
            Height = 70,
            Background = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["DesktopAcrylicTransparentBrush"],
            CornerRadius = new CornerRadius(5),
        };

        // Create ColumnDefinitions for the Grid
        documentContainer.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto }); // For image
        documentContainer.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) }); // For inner stack panel
        documentContainer.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto }); // For spacer
        documentContainer.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto }); // For font icon


        documentContainer.PointerEntered += (sender, e) =>
        {
            documentContainer.Background = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["LayerOnAcrylicFillColorDefaultBrush"];
        };

        documentContainer.PointerExited += (sender, e) =>
        {
            documentContainer.Background = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["DesktopAcrylicTransparentBrush"];
        };

        var radioButton = new RadioButton
        {
            Name = id.ToString(),
            IsChecked = false,
            Visibility = Visibility.Collapsed,
        };

        radioButton.Unchecked += (sender, e) =>
        {
            documentContainer.Background = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["DesktopAcrylicTransparentBrush"];

            documentContainer.PointerEntered += (sender, e) =>
            {
                documentContainer.Background = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["LayerOnAcrylicFillColorDefaultBrush"];
            };

            documentContainer.PointerExited += (sender, e) =>
            {
                documentContainer.Background = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["DesktopAcrylicTransparentBrush"];
            };
        };

        radioButtons.Items.Add(radioButton);

        // create document icon
        var image = new FontIcon
        {
            Glyph = "\uE8A5",
            FontSize = 35,
            Margin = new Thickness(12, 10, 10, 10)
        };

        // Create the inner StackPanel
        var innerStackPanel = new StackPanel
        {
            Orientation = Orientation.Vertical,
            Margin = new Thickness(5, 10, 0, 0)
        };

        // Title TextBlock
        var textBlock1 = new TextBlock
        {
            Text = title ?? "Custom",
            Style = (Style)Application.Current.Resources["BaseTextBlockStyle"],
            FontSize = 17
        };

        // Email TextBlock
        var textBlock2 = new TextBlock
        {
            Text = subtitle ?? "",
            Style = (Style)Application.Current.Resources["BaseTextBlockStyle"],
            FontSize = 12,
            Opacity = 0.9
        };

        // Add TextBlocks to the inner StackPanel
        innerStackPanel.Children.Add(textBlock1);
        innerStackPanel.Children.Add(textBlock2);
        innerStackPanel.Children.Add(radioButton);

        // Create a FontIcon
        var fontIcon = new FontIcon
        {
            Glyph = "\xE974",
            Opacity = 0.8,
            Margin = new Thickness(0, 0, 5, 0),
        };

        // Set Grid.Column for each element
        Grid.SetColumn(image, 0);
        Grid.SetColumn(innerStackPanel, 1);
        Grid.SetColumn(fontIcon, 3);

        // Add the Image, inner StackPanel, and FontIcon to the dynamic StackPanel
        documentContainer.Children.Add(image);
        documentContainer.Children.Add(innerStackPanel);
        documentContainer.Children.Add(fontIcon);

        documentContainer.PointerPressed += (sender, e) =>
        {
            foreach (var rb in radioButtons.Items.Cast<RadioButton>())
            {
                if (rb.Name == id.ToString())
                {
                    rb.IsChecked = true;
                    Debug.WriteLine(rb.Name);
                    //currentDocumentId = id ?? 0;

                    documentContainer.Background = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["LayerOnAcrylicFillColorDefaultBrush"];
                    documentContainer.PointerEntered += (sender, e) =>
                    {
                        documentContainer.Background = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["LayerOnAcrylicFillColorDefaultBrush"];
                    };
                    documentContainer.PointerExited += (sender, e) =>
                    {
                        documentContainer.Background = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["LayerOnAcrylicFillColorDefaultBrush"];
                    };

                    ErrorContainer_Grid.Visibility = Visibility.Collapsed;
                    AddDocumentContainer_Grid.Visibility = Visibility.Collapsed;
                    ViewDocument_Grid.Visibility = Visibility.Visible;

                    var document = DocumentDL.GetDocumentById(id ?? 0);

                    if (document == null)
                    {
                        return;
                    }

                    RenderUserInterface(document);
                }
                else
                {
                    rb.IsChecked = false;
                }
            }
        };

        DocumentsListView.Children.Add(documentContainer);
    }

    // Render User Interface: fill values in the document fields (View Document Page)
    private void RenderUserInterface(Document document)
    {
        Title_TextBox.Text = document.Title;

    }

    // Refresh Documents List View
    public void RefreshDocumentsListView()
    {
        DocumentsListView.Children.Clear();
        radioButtons.Items.Clear();

        if (DocumentDL.GetDocuments().Count == 0)
        {
            Documents_ScrollViewer.Visibility = Visibility.Collapsed;
            NoDocuments_Grid.Visibility = Visibility.Visible;
        }
        else
        {
            Documents_ScrollViewer.Visibility = Visibility.Visible;
            NoDocuments_Grid.Visibility = Visibility.Collapsed;

            var documents = DocumentDL.GetDocuments();

            foreach (var document in documents)
            {

                AddDocumentInListView(document.Id, document.Title, document.Type);

            }
        }
    }

    // View Add Document Page Button (Left Sidebar)
    private void AddDocument_Button_Click(object sender, RoutedEventArgs e)
    {
        ErrorContainer_Grid.Visibility = Visibility.Collapsed;
        AddDocumentContainer_Grid.Visibility = Visibility.Visible;
        ViewDocument_Grid.Visibility = Visibility.Collapsed;
    }

    // File Count Increase Button (Add Document Page)
    private void FileCountIncrease_Button_Click(object sender, RoutedEventArgs e)
    {
        filesCount += 1;

        var newButton = new Button
        {
            Name = $"File{filesCount}_Button",
            Width = 130,
            Height = 40,
            Margin = new Thickness(20, 8, 0, 0),
            Content = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Children =
                {
                    new FontIcon
                    {
                        FontSize = 17,
                        Glyph = "\uE8E5",
                    },
                    new TextBlock
                    {
                        Margin = new Thickness(5,0,0,0),
                        Text = "Browse File"
                    }
                }
            }
        };

        FileSelectButtons_Container.Children.Add(newButton);
        FileCountDecrease_Button.Visibility = Visibility.Visible;

        FileCountIncrease_Button.Visibility = (filesCount >= 5) ? Visibility.Collapsed : Visibility.Visible;
        FileCountDecrease_Button.Visibility = (filesCount <= 1) ? Visibility.Collapsed : Visibility.Visible;

    }

    // File Count Decrease Button (Add Document Page)
    private void FileCountDecrease_Button_Click(object sender, RoutedEventArgs e)
    {
        if (FileSelectButtons_Container.Children.Count > 0)
        {
            filesCount -= 1;
            FileSelectButtons_Container.Children.RemoveAt(FileSelectButtons_Container.Children.Count - 1);

            FileCountIncrease_Button.Visibility = (filesCount >= 5) ? Visibility.Collapsed : Visibility.Visible;
            FileCountDecrease_Button.Visibility = (filesCount <= 1) ? Visibility.Collapsed : Visibility.Visible;
        }
    }

    private async void File1_Button_Click(object sender, RoutedEventArgs e)
    {
        // Create a file picker
        var openPicker = new FileOpenPicker();

        // See the sample code below for how to make the window accessible from the App class.
        var window = App.MainWindow;

        // Retrieve the window handle (HWND) of the current WinUI 3 window.
        var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);

        // Initialize the file picker with the window handle (HWND).
        WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hWnd);

        // Set options for your file picker
        openPicker.ViewMode = PickerViewMode.Thumbnail;
        openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
        openPicker.FileTypeFilter.Add(".jpg");
        openPicker.FileTypeFilter.Add(".jpeg");
        openPicker.FileTypeFilter.Add(".png");

        // Open the picker for the user to pick a file
        var file = await openPicker.PickSingleFileAsync();
        if (file != null)
        {
            File1_Path.Text = file.Path;
        }

    }

    // Save Button (Add Document Page)
    private void Save_Button_Click(object sender, RoutedEventArgs e)
    {
        var Title = Title_TextBox.Text;
        var Type = DocumentType_ComboBox.SelectedValue.ToString();

        var document = new Document
        {
            Title = Title,
            Type = Type
        };

        DocumentDL.AddDocument(document);
        DocumentDB.StoreDocument(document);

        var filePath = File1_Path.Text;
        var fileName = Path.GetFileNameWithoutExtension(filePath);
        var fileType = Path.GetExtension(filePath);

        var documentFile = new DocumentFile
        {
            DocumentId = DocumentDB.GetMaxDocumentId(),
            FileName = fileName,
            FileType = fileType,
            FileContent = File.ReadAllBytes(filePath)
        };

        DocumentDL.AddDocumentFile(documentFile);
        DocumentDB.StoreDocumentFile(documentFile);

        AddDocumentInListView(document.Id, document.Title, document.Type);
        MessageDialogHelper.ShowMessageDialog(XamlRoot, "Success", "Document added successfully.");
    }

    private void DocumentType_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (CustomType_TextBox != null)
        {
            if (DocumentType_ComboBox.SelectedValue.ToString() == "Custom")
            {
                CustomType_TextBox.Visibility = Visibility.Visible;
            }
            else
            {
                CustomType_TextBox.Visibility = Visibility.Collapsed;
            }
        }
    }
}