using Cyber_Vault.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Cyber_Vault.BL;
using Cyber_Vault.DL;
using Cyber_Vault.DB;
using Microsoft.UI.Xaml.Controls.Primitives;
using CommunityToolkit.Mvvm.Input;
using Windows.ApplicationModel.DataTransfer;
using Cyber_Vault.Utils;
using System.Diagnostics;
using QRCoder;
using OtpNet;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Text;
using Microsoft.UI.Xaml.Input;

namespace Cyber_Vault.Views;

public sealed partial class DocumentsPage : Page
{
    private int currentDocumentId = 0;
    private int _selectedDocumentIndex = -1;
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

        // Create an Image and set its properties
       /* if (url != null && url != string.Empty)
        {
            url = $"https://www.google.com/s2/favicons?domain={url}&sz=128";
        }
        else
        {
            url = "https://www.unsplash.com";
        }

        var image = new Image
        {
            Margin = new Thickness(12, 10, 10, 10),
            Source = new BitmapImage(new Uri(url)),
            Width = 35,
            Height = 35
        };*/
       // create document icon
       var image = new FontIcon
       {
       
                  Glyph = "\uE8A5",
                  FontSize = 35,
                  Margin = new Thickness(12, 10, 10, 10),
                  Foreground = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["SystemAccentColor"],
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
                    currentDocumentId = id ?? 0;
                    OTP_Ring.Value = 100;
                   /* timer?.Dispose();*/
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

}