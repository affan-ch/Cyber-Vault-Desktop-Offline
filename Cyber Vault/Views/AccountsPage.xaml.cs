using Cyber_Vault.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Media;

namespace Cyber_Vault.Views;

public sealed partial class AccountsPage : Page
{
    public AccountsViewModel ViewModel
    {
        get;
    }

    public AccountsPage()
    {
        ViewModel = App.GetService<AccountsViewModel>();
        InitializeComponent();

        AddAccountInListView("https://www.google.com/s2/favicons?domain=microsoft.com&sz=128", "Microsoft", "affan_ali_ch@outlook.com");
        AddAccountInListView("https://www.google.com/s2/favicons?domain=google.com&sz=128", "Google", "affan@gmail.com");
        AddAccountInListView("https://www.google.com/s2/favicons?domain=google.com&sz=128", "Google", "affan@gmail.com");
        AddAccountInListView("https://www.google.com/s2/favicons?domain=google.com&sz=128", "Google", "affan@gmail.com");
        AddAccountInListView("https://www.google.com/s2/favicons?domain=google.com&sz=128", "Google", "affan@gmail.com");
        AddAccountInListView("https://www.google.com/s2/favicons?domain=google.com&sz=128", "Google", "affan@gmail.com");
        AddAccountInListView("https://www.google.com/s2/favicons?domain=google.com&sz=128", "Google", "affan@gmail.com");
        AddAccountInListView("https://www.google.com/s2/favicons?domain=google.com&sz=128", "Google", "affan@gmail.com");
        AddAccountInListView("https://www.google.com/s2/favicons?domain=microsoft.com&sz=128", "Microsoft", "affan_ali_ch@outlook.com");


    }

    private void AddAccountInListView(string url, string title, string email)
    {

        // Create a new StackPanel dynamically
        var dynamicStackPanel = new Grid
        {
            Margin = new Thickness(0, 1, 20, 5),
            Height = 70,
            Background = (Brush)Application.Current.Resources["DesktopAcrylicTransparentBrush"],
            CornerRadius = new CornerRadius(5),
            
        };

        // Create ColumnDefinitions for the Grid
        dynamicStackPanel.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto }); // For image
        dynamicStackPanel.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) }); // For inner stack panel
        dynamicStackPanel.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto }); // For spacer
        dynamicStackPanel.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto }); // For font icon


        dynamicStackPanel.PointerEntered += (sender, e) =>
        {
            dynamicStackPanel.Background = (Brush)Application.Current.Resources["LayerOnAcrylicFillColorDefaultBrush"];
        };

        dynamicStackPanel.PointerExited += (sender, e) =>
        {
             dynamicStackPanel.Background = (Brush)Application.Current.Resources["DesktopAcrylicTransparentBrush"];
        };

        // Create an Image and set its properties
        var image = new Image
        {
            Margin = new Thickness(15, 10, 10, 10),
            Source = new BitmapImage(new Uri(url)),
            Width = 40,
            Height = 40
        };

        // Create the inner StackPanel
        var innerStackPanel = new StackPanel
        {
            Orientation = Orientation.Vertical,
            Margin = new Thickness(5, 10, 0, 0)
        };

        // Create TextBlocks and set their properties
        var textBlock1 = new TextBlock
        {
            Text = title,
            Style = (Style)Application.Current.Resources["BaseTextBlockStyle"],
            FontSize = 17
        };

        TextBlock textBlock2 = new TextBlock
        {
            Text = email,
            Style = (Style)Application.Current.Resources["BaseTextBlockStyle"],
            FontSize = 14,
            Opacity = 0.9
        };

        // Add TextBlocks to the inner StackPanel
        innerStackPanel.Children.Add(textBlock1);
        innerStackPanel.Children.Add(textBlock2);


        // Create a FontIcon
        var fontIcon = new FontIcon
        {
            Glyph = "\xE974",
            Opacity = 0.8,
            Margin = new Thickness(0, 0, 10, 0),
        };

        // Set Grid.Column for each element
        Grid.SetColumn(image, 0);
        Grid.SetColumn(innerStackPanel, 1);
        Grid.SetColumn(fontIcon, 3);

        // Add the Image, inner StackPanel, and FontIcon to the dynamic StackPanel
        dynamicStackPanel.Children.Add(image);
        dynamicStackPanel.Children.Add(innerStackPanel);
        dynamicStackPanel.Children.Add(fontIcon);

        AccountsListView.Children.Add(dynamicStackPanel);
    }
}
