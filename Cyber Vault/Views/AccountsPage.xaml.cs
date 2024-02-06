﻿using Cyber_Vault.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Media;
using Cyber_Vault.BL;
using Cyber_Vault.DL;
using Cyber_Vault.DB;
using Microsoft.UI.Xaml.Controls.Primitives;
using CommunityToolkit.Mvvm.Input;
using Windows.ApplicationModel.DataTransfer;
using Cyber_Vault.Utils;


namespace Cyber_Vault.Views;

public sealed partial class AccountsPage : Page
{
    private int backupCodeCount = 1;

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

        var backupCodes = new List<BackupCode>();
        foreach (var child in BackupCodes_StackPanel.Children)
        {
            if (child is TextBox backupCode)
            {
                if(backupCode.Text == string.Empty)
                {
                    continue;
                }
            
                var backupCodeBL = new BackupCode
                (
                    AccountId: AccountDB.GetMaxId(),
                    Code: backupCode.PlaceholderText
                );
            
                BackupCodeDL.AddBackupCode(backupCodeBL);
                backupCodes.Add(backupCodeBL);
            }
        }
        BackupCodeDB.StoreBackupCodes(backupCodes);


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

    private void AddBackupCode_Button_Click(object sender, RoutedEventArgs e)
    {
        backupCodeCount += 1;


        var random = new Random();

        var backupCode = new TextBox
        {
            Name = $"BackupCode{backupCodeCount}_TextBox",
            Margin = new Thickness(20, 20, 13, 0),
            PlaceholderText = random.Next(100000, 1000000).ToString(),
            Header = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Children =
                {
                    new TextBlock
                    {
                        Text = $"Enter Backup Code {backupCodeCount}",
                        Margin = new Thickness(0, 0, 4, 0)
                    },
                    new FontIcon
                    {
                        Glyph = "\xE734",
                        FontSize = 13
                    }
                }
            }
        };

        BackupCodes_StackPanel.Children.Add(backupCode);

        AddBackupCode_Button.Visibility = (backupCodeCount >= 16) ? Visibility.Collapsed : Visibility.Visible;
        RemoveBackupCode_Button.Visibility = (backupCodeCount <= 1) ? Visibility.Collapsed : Visibility.Visible;
    }

    private void RemoveBackupCode_Button_Click(object sender, RoutedEventArgs e)
    {
        if (BackupCodes_StackPanel.Children.Count > 0)
        {
            backupCodeCount -= 1;
            BackupCodes_StackPanel.Children.RemoveAt(BackupCodes_StackPanel.Children.Count - 1);

            AddBackupCode_Button.Visibility = (backupCodeCount >= 16) ? Visibility.Collapsed : Visibility.Visible;
            RemoveBackupCode_Button.Visibility = (backupCodeCount <= 1) ? Visibility.Collapsed : Visibility.Visible;
        }
    }
    

    // Password Generator
    private async void GeneratePassword_Button_Click(object _, RoutedEventArgs e)
    {
        // UpperCase CheckBox
        var UpperCase_CheckBox = new CheckBox
        {
            Name = "UpperCase_CheckBox",
            Content = "Include Uppercase (A-Z)",
            Margin = new Thickness(0, 0, 0, 10),
            IsChecked = true
        };

        // LowerCase CheckBox
        var LowerCase_CheckBox = new CheckBox
        {
            Name = "LowerCase_CheckBox",
            Content = "Include Lowercase (a-z)",
            Margin = new Thickness(0, 0, 0, 10),
            IsChecked = true
        };

        // Numbers CheckBox
        var Numbers_CheckBox = new CheckBox
        {
            Name = "Numbers_CheckBox",
            Content = "Include Numbers (0-9)",
            Margin = new Thickness(0, 0, 0, 10),
            IsChecked = true
        };

        // SpecialCharacters CheckBox
        var SpecialCharacters_CheckBox = new CheckBox
        {
            Name = "SpecialCharacters_CheckBox",
            Content = "Include Special Characters (!@#$%&?)",
            Margin = new Thickness(0, 0, 0, 10),
            IsChecked = true
        };

        // Password Length Slider
        var PasswordLength_Slider = new Slider
        {
            Name = "PasswordLength_Slider",
            Header = "Password Length",
            Minimum = 8,
            Maximum = 64,
            Value = 20,
            Margin = new Thickness(0, 0, 0, 10)
        };

        // Output Grid
        var Output_Grid = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                new ColumnDefinition { Width = GridLength.Auto },
                new ColumnDefinition { Width = GridLength.Auto }
            },
        };

        // Generated Password TextBox
        var GeneratedPassword_TextBox = new TextBox
        {
            Name = "GeneratedPassword_TextBox",
            Header = "Generated Password",
            IsReadOnly = true,
            Margin = new Thickness(0, 0, 0, 10),
            MinWidth = 300,
            Text = PasswordGenerator.Generate(20, true, true, true, true),
            TextWrapping = TextWrapping.Wrap,
        };

        // Copy Password Button
        var CopyPassword_Button = new Button
        {
            Name = "CopyPassword_Button",
            Content = new FontIcon
            {
                Glyph = "\xE8C8",
                FontSize = 15
            },
            Margin = new Thickness(10, 0, 0, 10),
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Bottom,
            Height = 32,
            Command = new RelayCommand(() =>
            {
                var textBox = GeneratedPassword_TextBox;
                var dataPackage = new DataPackage();
                dataPackage.SetText(textBox.Text);
                Clipboard.SetContent(dataPackage);
            })
        };

        // Set ToolTips for the Copy Password Button
        ToolTipService.SetToolTip(CopyPassword_Button, "Copy Password");
        ToolTipService.SetPlacement(CopyPassword_Button, PlacementMode.Bottom);

        // Generate Password Button
        var GeneratePassword_Button = new Button
        {
            Name = "GeneratePassword_Button",
            Content = new FontIcon
            {
                Glyph = "\uE777",
                FontSize = 14
            },
            Margin = new Thickness(10, 0, 0, 10),
            HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Bottom,
            Height = 32,
            Command = new RelayCommand(() =>
            {   
                var includeUpper = UpperCase_CheckBox.IsChecked ?? false;
                var includeLower = LowerCase_CheckBox.IsChecked ?? false;
                var includeNumbers = Numbers_CheckBox.IsChecked ?? false;
                var includeSpecialChars = SpecialCharacters_CheckBox.IsChecked ?? false;
                var length = (int)PasswordLength_Slider.Value;
                       
                var password = PasswordGenerator.Generate(length, includeUpper, includeLower, includeNumbers, includeSpecialChars);
                GeneratedPassword_TextBox.Text = password;
            })
        };

        // Set ToolTips for the Generate Password Button
        ToolTipService.SetToolTip(GeneratePassword_Button, "Generate New Password");
        ToolTipService.SetPlacement(GeneratePassword_Button, PlacementMode.Bottom);

        // Set the Grid.Column for each element
        Grid.SetColumn(GeneratedPassword_TextBox, 0);
        Grid.SetColumn(CopyPassword_Button, 1);
        Grid.SetColumn(GeneratePassword_Button, 2);

        // Add the elements to the Output Grid
        Output_Grid.Children.Add(GeneratedPassword_TextBox);
        Output_Grid.Children.Add(CopyPassword_Button);
        Output_Grid.Children.Add(GeneratePassword_Button);


        // StackPanel to contain the elements
        var Container_StackPanel = new StackPanel 
        { 
            Orientation = Orientation.Vertical 
        };

        // Add the elements to the StackPanel
        Container_StackPanel.Children.Add(UpperCase_CheckBox);
        Container_StackPanel.Children.Add(LowerCase_CheckBox);
        Container_StackPanel.Children.Add(Numbers_CheckBox);
        Container_StackPanel.Children.Add(SpecialCharacters_CheckBox);
        Container_StackPanel.Children.Add(PasswordLength_Slider);
        Container_StackPanel.Children.Add(Output_Grid);


        var dialog = new ContentDialog
        {
            XamlRoot = XamlRoot,
            Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
            Title = "Generate Password",
            PrimaryButtonText = "Use this Password",
            CloseButtonText = "Cancel",
            DefaultButton = ContentDialogButton.Primary,
            Content = Container_StackPanel
        };

        var result = await dialog.ShowAsync();

        if(result.ToString() == "Primary")
        {
            Password_TextBox.Password = GeneratedPassword_TextBox.Text;
        }

    }

}
