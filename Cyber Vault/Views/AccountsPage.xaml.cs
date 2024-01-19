using Cyber_Vault.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Media;
using Cyber_Vault.BL;
using Cyber_Vault.DL;
using Cyber_Vault.DB;

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

        if(AccountDL.GetAccounts().Count == 0)
        {
            Accounts_ScrollViewer.Visibility = Visibility.Collapsed;
            NoAccounts_Grid.Visibility = Visibility.Visible;
        }
        else
        {
            Accounts_ScrollViewer.Visibility = Visibility.Visible;
            NoAccounts_Grid.Visibility = Visibility.Collapsed;

            var accounts = AccountDL.GetAccounts();

            foreach(var account in accounts)
            {
                AddAccountInListView($"https://www.google.com/s2/favicons?domain={account.Domain}&sz=128", account.Title!, account.Email!);
            }
        }

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
            Margin = new Thickness(12, 10, 10, 10),
            Source = new BitmapImage(new Uri(url)),
            Width = 35,
            Height = 35
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
            Text = title,
            Style = (Style)Application.Current.Resources["BaseTextBlockStyle"],
            FontSize = 17
        };

        // Email TextBlock
        var textBlock2 = new TextBlock
        {
            Text = email,
            Style = (Style)Application.Current.Resources["BaseTextBlockStyle"],
            FontSize = 12,
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
            Margin = new Thickness(0, 0, 5, 0),
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

    private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (Title_TextBox != null && Domain_TextBox != null)
        {
            if (AccountType_ComboBox.SelectedValue.ToString() == "Custom")
            {
                Title_TextBox.Visibility = Visibility.Visible;
                Domain_TextBox.Visibility = Visibility.Visible;

            }
            else
            {
                Title_TextBox.Visibility = Visibility.Collapsed;
                Domain_TextBox.Visibility = Visibility.Collapsed;
            }
        } 
    }

    private void AddAccount_Button_Click(object sender, RoutedEventArgs e)
    {
        ErrorContainer_Grid.Visibility = Visibility.Collapsed;
        AddAccountContainer_Grid.Visibility = Visibility.Visible;

    }

    private void Save_Button_Click(object sender, RoutedEventArgs e)
    {
        var AccountType = AccountType_ComboBox.SelectedValue.ToString();
        var Title = Title_TextBox.Text;
        var Domain = Domain_TextBox.Text;
        var Name = Name_TextBox.Text;
        var Email = Email_TextBox.Text;
        var Username = Username_TextBox.Text;
        var PhoneNumber = PhoneNumber_TextBox.Text;
        var Password = Password_TextBox.Password;
        var Pin = Pin_TextBox.Password;
        var DateOfBirth = DateOfBirth_TextBox.Text;
        var RecoveryEmail = RecoveryEmail_TextBox.Text;
        var RecoveryPhoneNumber = RecoveryPhoneNumber_TextBox.Text;
        var QrCode = QrCode_TextBox.Text;
        var SecretKey = SecretKey_TextBox.Text;
        var Notes = Notes_TextBox.Text;

        if (AccountType == "Custom")
        {
            if (Title == string.Empty || Domain == string.Empty)
            {    
                // TODO: Show error message
                return;
            }
        }
        else
        {
            
        }

        var account = new Account
        (
            Type: AccountType ?? "",
            Title: Title ?? "",
            Domain: Domain ?? "",
            Name: Name ?? "",
            Email: Email ?? "",
            Username: Username ?? "",
            PhoneNumber: PhoneNumber ?? "",
            Password: Password ?? "",
            Pin: Pin ?? "",
            DateOfBirth: DateOfBirth ?? "",
            RecoveryEmail: RecoveryEmail ?? "",
            RecoveryPhoneNumber: RecoveryPhoneNumber ?? "",
            QrCode: QrCode ?? "",
            Secret: SecretKey ?? "",
            Notes: Notes ?? ""
        );

        AccountDL.AddAccount(account);
        AccountDB.StoreAccount(account);
        // TODO: Show success message
        ClearFields();
    }

    private void ClearFields()
    {
        Title_TextBox.Text = string.Empty;
        Domain_TextBox.Text = string.Empty;
        Name_TextBox.Text = string.Empty;
        Email_TextBox.Text = string.Empty;
        Username_TextBox.Text = string.Empty;
        PhoneNumber_TextBox.Text = string.Empty;
        Password_TextBox.Password = string.Empty;
        Pin_TextBox.Password = string.Empty;
        DateOfBirth_TextBox.Text = string.Empty;
        RecoveryEmail_TextBox.Text = string.Empty;
        RecoveryPhoneNumber_TextBox.Text = string.Empty;
        QrCode_TextBox.Text = string.Empty;
        SecretKey_TextBox.Text = string.Empty;
        Notes_TextBox.Text = string.Empty;
    }
}
