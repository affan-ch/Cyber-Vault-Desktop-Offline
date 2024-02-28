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

public sealed partial class AccountsPage : Page
{
    private int currentAccountId = 0;
    private int backupCodeCount = 1;
    private string oldOTP = "";
    private Timer? timer;
    private Authenticator? currentAuthenticator;
    private readonly RadioButtons radioButtons = new()
    {
        Visibility = Visibility.Collapsed
    };

    public AccountsViewModel ViewModel
    {
        get;
    }

    public AccountsPage()
    {
        ViewModel = App.GetService<AccountsViewModel>();
        InitializeComponent();
        RefreshAccountsListView();
    }

    // Add Account Tile in ListView (Left Sidebar)
    private void AddAccountInListView(int? id, string? url, string? title, string? subtitle)
    {
        NoAccounts_Grid.Visibility = Visibility.Collapsed;

        // Create a new StackPanel dynamically
        var accountContainer = new Grid
        {
            Margin = new Thickness(0, 1, 20, 5),
            Height = 70,
            Background = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["DesktopAcrylicTransparentBrush"],
            CornerRadius = new CornerRadius(5),
        };

        // Create ColumnDefinitions for the Grid
        accountContainer.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto }); // For image
        accountContainer.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) }); // For inner stack panel
        accountContainer.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto }); // For spacer
        accountContainer.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto }); // For font icon


        accountContainer.PointerEntered += (sender, e) =>
        {
            accountContainer.Background = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["LayerOnAcrylicFillColorDefaultBrush"];
        };

        accountContainer.PointerExited += (sender, e) =>
        {
             accountContainer.Background = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["DesktopAcrylicTransparentBrush"];
        };

        var radioButton = new RadioButton
        {
            Name = id.ToString(),
            IsChecked = false,
            Visibility = Visibility.Collapsed,
        };

        radioButton.Unchecked += (sender, e) =>
        {
            accountContainer.Background = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["DesktopAcrylicTransparentBrush"];
            
            accountContainer.PointerEntered += (sender, e) =>
            {
                accountContainer.Background = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["LayerOnAcrylicFillColorDefaultBrush"];
            };

            accountContainer.PointerExited += (sender, e) =>
            {
                accountContainer.Background = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["DesktopAcrylicTransparentBrush"];
            };
        };

        radioButtons.Items.Add(radioButton);

        // Create an Image and set its properties
        if(url != null && url != string.Empty)
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
        accountContainer.Children.Add(image);
        accountContainer.Children.Add(innerStackPanel);
        accountContainer.Children.Add(fontIcon);

        accountContainer.PointerPressed += (sender, e) =>
        {
            foreach (var rb in radioButtons.Items.Cast<RadioButton>())
            {
                if (rb.Name == id.ToString())
                {
                    rb.IsChecked = true;
                    Debug.WriteLine(rb.Name);
                    currentAccountId = id ?? 0;
                    OTP_Ring.Value = 100;
                    timer?.Dispose();
                    accountContainer.Background = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["LayerOnAcrylicFillColorDefaultBrush"];
                    accountContainer.PointerEntered += (sender, e) =>
                    {
                        accountContainer.Background = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["LayerOnAcrylicFillColorDefaultBrush"];
                    };
                    accountContainer.PointerExited += (sender, e) =>
                    {                    
                        accountContainer.Background = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["LayerOnAcrylicFillColorDefaultBrush"];
                    };

                    ErrorContainer_Grid.Visibility = Visibility.Collapsed;
                    AddAccountContainer_Grid.Visibility = Visibility.Collapsed;
                    ViewAccount_Grid.Visibility = Visibility.Visible;

                    var account = AccountDL.GetAccountById(id ?? 0);

                    if (account == null)
                    {
                        return;
                    }

                    RenderUserInterface(account);
                }
                else
                {
                    rb.IsChecked = false;
                }
            }
        };

        AccountsListView.Children.Add(accountContainer);
    }

    // Method to update OTP (View Account Page - Authenticator)
    private void UpdateOTP(Authenticator authenticator)
    {
        var period = authenticator.Period;
        if (string.IsNullOrEmpty(period))
        {
            period = "30";
        }

        var counter = authenticator.Counter;
        if (string.IsNullOrEmpty(counter))
        {
            counter = "0";
        }

        var algo = authenticator.Algorithm;
        if (string.IsNullOrEmpty(algo))
        {
            algo = "SHA1";
        }
        algo = algo.ToUpper();

        var digits = authenticator.Digits;
        if (string.IsNullOrEmpty(digits))
        {
            digits = "6";
        }

        if (authenticator.Type!.ToUpper() == "TOTP")
        {
            var obj = new Totp(
                secretKey: Base32Encoding.ToBytes(authenticator.Secret),
                step: int.Parse(period),
                mode: algo == "SHA1" ? OtpHashMode.Sha1 : algo == "SHA256" ? OtpHashMode.Sha256 : OtpHashMode.Sha512,
                totpSize: int.Parse(digits),
                timeCorrection: TimeCorrection.UncorrectedInstance
            );

            var otp = obj.ComputeTotp(DateTime.UtcNow);
            var remainingSeconds = obj.RemainingSeconds(DateTime.UtcNow);

            if (oldOTP == "")
            {
                oldOTP = otp;
                SetProgressRingValue(remainingSeconds, period);
                DispatcherQueue.TryEnqueue(() =>
                {
                    Authenticator_Text.Text = otp;
                });
            }
            else if (oldOTP != otp)
            {
                DispatcherQueue.TryEnqueue(() =>
                {
                    Authenticator_Text.Text = otp;
                });
                SetProgressRingValue(remainingSeconds, period);
            }
            else if (oldOTP == otp)
            {
                SetProgressRingValue(remainingSeconds, period);
            }

        }
        else
        {        
            return;
        }

    }

    // Method to set the progress ring value (View Account Page)
    private void SetProgressRingValue(int remainingSeconds, string period)
    {
        // Calculate the progress ring value based on remaining seconds
        var progressValue = (remainingSeconds / double.Parse(period)) * 100.0;

        // Set the progress ring value
        DispatcherQueue.TryEnqueue(() => {
            OTP_Ring.Value = progressValue;
        });
    }

    // Timer callback method (View Account Page - Authenticator Timer)
    private void TimerCallback(object? state)
    {
        if(currentAuthenticator != null)
        {
            UpdateOTP(currentAuthenticator);
        }
    }

    // Fill Values in the Account Fields (View Account Page)
    private void RenderUserInterface(Account account)
    {

        // Domain
        if (account.Domain != null && account.Domain != string.Empty)
        {
            Domain_Container.Visibility = Visibility.Visible;
            Domain_Text.Text = account.Domain;
        }
        else
        {
            Domain_Container.Visibility = Visibility.Collapsed;
        }

        // Name
        if (account.Name != null && account.Name != string.Empty)
        {
            Name_Container.Visibility = Visibility.Visible;
            Name_Text.Text = account.Name;
        }
        else
        {
            Name_Container.Visibility = Visibility.Collapsed;
        }

        // Email
        if (account.Email != null && account.Email != string.Empty)
        {
            Email_Container.Visibility = Visibility.Visible;
            Email_Text.Text = account.Email;
        }
        else
        {
            Email_Container.Visibility = Visibility.Collapsed;
        }

        // Username
        if (account.Username != null && account.Username != string.Empty)
        {
            Username_Container.Visibility = Visibility.Visible;
            Username_Text.Text = account.Username;
        }
        else
        {
            Username_Container.Visibility = Visibility.Collapsed;
        }

        // Phone Number
        if (account.PhoneNumber != null && account.PhoneNumber != string.Empty)
        {
            PhoneNumber_Container.Visibility = Visibility.Visible;
            PhoneNumber_Text.Text = account.PhoneNumber;
        }
        else
        {
            PhoneNumber_Container.Visibility = Visibility.Collapsed;
        }

        // Password
        if (account.Password != null && account.Password != string.Empty)
        {
            Password_Container.Visibility = Visibility.Visible;
            Password_Text.Text = new string('●', account.Password.Length);
            Password_Text_Hidden.Text = account.Password;
            TogglePassword_CheckBox.IsChecked = false;
            PasswordToggle_Icon.Glyph = "\uE7B3";
            ToolTipService.SetToolTip(TogglePassword_Button, "Show Password");
        }
        else
        {
            Password_Container.Visibility = Visibility.Collapsed;
        }

        // Pin
        if (account.Pin != null && account.Pin != string.Empty)
        {
            Pin_Container.Visibility = Visibility.Visible;
            Pin_Text.Text = new string('●', account.Pin.Length);
            Pin_Text_Hidden.Text = account.Pin;
            TogglePin_CheckBox.IsChecked = false;
            PinToggle_Icon.Glyph = "\uE7B3";
            ToolTipService.SetToolTip(TogglePin_Button, "Show Pin");
        }
        else
        {
            Pin_Container.Visibility = Visibility.Collapsed;
        }

        // Date of Birth
        if (account.DateOfBirth != null && account.DateOfBirth != string.Empty)
        {
            DOB_Container.Visibility = Visibility.Visible;
            DOB_Text.Text = account.DateOfBirth;
        }
        else
        {
            DOB_Container.Visibility = Visibility.Collapsed;
        }

        // Notes
        if (account.Notes != null && account.Notes != string.Empty)
        {
            Notes_Container.Visibility = Visibility.Visible;
            Notes_Text.Text = account.Notes;
        }
        else
        {
            Notes_Container.Visibility = Visibility.Collapsed;
        }

        // Recovery Email
        if (account.RecoveryEmail != null && account.RecoveryEmail != string.Empty)
        {
            RecoveryEmail_Container.Visibility = Visibility.Visible;
            RecoveryEmail_Text.Text = account.RecoveryEmail;
        }
        else
        {
            RecoveryEmail_Container.Visibility = Visibility.Collapsed;
        }

        // Recovery Phone Number
        if (account.RecoveryPhoneNumber != null && account.RecoveryPhoneNumber != string.Empty)
        {
            RecoveryPhoneNumber_Container.Visibility = Visibility.Visible;
            RecoveryPhoneNumber_Text.Text = account.RecoveryPhoneNumber;
        }
        else
        {
            RecoveryPhoneNumber_Container.Visibility = Visibility.Collapsed;
        }

        // QR Code
        if ((account.QrCode != null && account.QrCode != string.Empty))
        {
            Authenticator_Container.Visibility = Visibility.Visible;

            var authenticator = ParseUri(account.QrCode);
            
            if(authenticator != null)
            {
                if (!string.IsNullOrEmpty(authenticator.Secret) && !string.IsNullOrEmpty(authenticator.Type))
                {
                    currentAuthenticator = authenticator;

                    if(authenticator.Type.ToUpper() == "TOTP")
                    {
                        OTP_Ring.Visibility = Visibility.Visible;
                        HOTP_Generate_Button.Visibility = Visibility.Collapsed;

                        // Timer to update the OTP and ProgressRing value
                        timer = new Timer(TimerCallback, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(300));
                    }
                    else if(authenticator.Type.ToUpper() == "HOTP")
                    {
                        OTP_Ring.Visibility = Visibility.Collapsed;
                        HOTP_Generate_Button.Visibility = Visibility.Visible;
                        Authenticator_Text.Text = new string('●', int.Parse(authenticator.Digits ?? "6"));
                    }
                    
                }
            }
        }
        else
        {
            currentAuthenticator = null;
            Authenticator_Container.Visibility = Visibility.Collapsed;
        }

        // Backup Codes
        var backupCodes = BackupCodeDL.GetBackupCodesByAccountId(account.Id ?? 0);
        if (backupCodes.Count > 0)
        {
            BackupCodes_Grids.Children.Clear();
            BackupCodes_Container.Visibility = Visibility.Visible;
            foreach (var backupCode in backupCodes)
            {
                AddBackupCodeinContainer(backupCode.Code ?? "", backupCode.IsUsed ?? 0, account.Id ?? 0);
            }
        }
        else
        {
            BackupCodes_Container.Visibility = Visibility.Collapsed;
        }

    }

    // Account Type ComboBox Selection Changed
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

    // Add Account Button (Left Sidebar)
    private void AddAccount_Button_Click(object sender, RoutedEventArgs e)
    {
        ViewAccount_Grid.Visibility = Visibility.Collapsed;
        ErrorContainer_Grid.Visibility = Visibility.Collapsed;
        AddAccountContainer_Grid.Visibility = Visibility.Visible;

        ClearFields();
        Update_Button.Visibility = Visibility.Collapsed;
        Save_Button.Visibility = Visibility.Visible;

        foreach (var rb in radioButtons.Items.Cast<RadioButton>())
        {
            rb.IsChecked = false;
        }
    }

    // Save Account Button (Add Account Page)
    private void Save_Button_Click(object sender, RoutedEventArgs e)
    {
        var account = GetInputAccount();

        if(account == null)
        {        
            return;
        }

        AccountDL.AddAccount(account);
        AccountDB.StoreAccount(account);
        if (!string.IsNullOrEmpty(account.Email))
        {
            AddAccountInListView(account.Id, account.Domain, account.Title, account.Email);
        }
        else if (!string.IsNullOrEmpty(account.Username))
        {
            AddAccountInListView(account.Id, account.Domain, account.Title, account.Username);
        }
        else if (!string.IsNullOrEmpty(account.PhoneNumber))
        {
            AddAccountInListView(account.Id, account.Domain, account.Title, account.PhoneNumber);
        }
        else
        {
            AddAccountInListView(account.Id, account.Domain, account.Title, string.Empty);
        }

        var backupCodes = GetInputBackupCodes();
        backupCodes.ForEach(bc => bc.AccountId = account.Id);
        backupCodes.ForEach(bc => BackupCodeDL.AddBackupCode(bc));
        BackupCodeDB.StoreBackupCodes(backupCodes);

        ClearFields();
        MessageDialogHelper.ShowMessageDialog(XamlRoot, "Success", "Account added successfully!");
    }

    // Clear All Fields (Add Account Page)
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
        Notes_TextBox.Text = string.Empty;

        RemoveAllBackupCodesTextbox();
    }

    // Add Backup Code Field (Add Account Page)
    private void AddBackupCode_Button_Click(object? _, RoutedEventArgs? e)
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

    // Remove Last Backup Code Field (Add Account Page)
    private void RemoveBackupCode_Button_Click(object _, RoutedEventArgs e)
    {
        if (BackupCodes_StackPanel.Children.Count > 0)
        {
            backupCodeCount -= 1;
            BackupCodes_StackPanel.Children.RemoveAt(BackupCodes_StackPanel.Children.Count - 1);

            AddBackupCode_Button.Visibility = (backupCodeCount >= 16) ? Visibility.Collapsed : Visibility.Visible;
            RemoveBackupCode_Button.Visibility = (backupCodeCount <= 1) ? Visibility.Collapsed : Visibility.Visible;
        }
    }

    private void RemoveAllBackupCodesTextbox()
    {
        for(var i = BackupCodes_StackPanel.Children.Count - 1; i >= 1; i--)
        {
            BackupCodes_StackPanel.Children.RemoveAt(i);
        }
        backupCodeCount = 1;
        AddBackupCode_Button.Visibility = Visibility.Visible;
        RemoveBackupCode_Button.Visibility = Visibility.Collapsed;
        BackupCode1_TextBox.Text = string.Empty;
    }
    
    // Password Generator (Add Account Page)
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

    // Open Domain Link in Browser (View Account Page)
    private void OpenDomainLink_Button_Click(object _, RoutedEventArgs e)
    {
        var domain = Domain_Text.Text;
        if(domain != string.Empty)
        {
            Process.Start(new ProcessStartInfo("https://" + domain) { UseShellExecute = true });
        }
    }

    // Copy Domain Button (View Account Page)
    private void CopyDomain_Button_Click(object _, RoutedEventArgs e)
    {
        var dataPackage = new DataPackage();
        dataPackage.SetText(Domain_Text.Text);
        Clipboard.SetContent(dataPackage);
    }

    // Copy Email Button (View Account Page)
    private void CopyEmail_Button_Click(object _, RoutedEventArgs e)
    {
        var dataPackage = new DataPackage();
        dataPackage.SetText(Email_Text.Text);
        Clipboard.SetContent(dataPackage);
    }

    // Copy Username Button (View Account Page)
    private void CopyUsername_Button_Click(object _, RoutedEventArgs e)
    {
        var dataPackage = new DataPackage();
        dataPackage.SetText(Username_Text.Text);
        Clipboard.SetContent(dataPackage);
    }

    // Copy Phone Number Button (View Account Page)
    private void CopyPhoneNumber_Button_Click(object _, RoutedEventArgs e)
    {
        var dataPackage = new DataPackage();
        dataPackage.SetText(PhoneNumber_Text.Text);
        Clipboard.SetContent(dataPackage);
    }

    // Toggle Hide/Unhide Password Button (View Account Page)
    private void TogglePassword_Button_Click(object _, RoutedEventArgs e)
    {
        if(TogglePassword_CheckBox.IsChecked == false)
        {
            Password_Text.Text = Password_Text_Hidden.Text;
            TogglePassword_CheckBox.IsChecked = true;
            ToolTipService.SetToolTip(TogglePassword_Button, "Hide Password");
            PasswordToggle_Icon.Glyph = "\uED1A";
        }
        else
        {
            Password_Text.Text = new string('●', Password_Text_Hidden.Text.Length);
            TogglePassword_CheckBox.IsChecked = false;
            ToolTipService.SetToolTip(TogglePassword_Button, "Show Password");
            PasswordToggle_Icon.Glyph = "\uE7B3";
        }
    }

    // Toggle Hide/Unhide Pin Button (View Account Page)
    private void TogglePin_Button_Click(object _, RoutedEventArgs e)
    {
        if (TogglePin_CheckBox.IsChecked == false)
        {
            TogglePin_CheckBox.IsChecked = true;
            ToolTipService.SetToolTip(TogglePin_Button, "Hide Pin");
            PinToggle_Icon.Glyph = "\uED1A";
            Pin_Text.Text = Pin_Text_Hidden.Text;
        }
        else
        {
            TogglePin_CheckBox.IsChecked = false;
            ToolTipService.SetToolTip(TogglePin_Button, "Show Pin");
            PinToggle_Icon.Glyph = "\uE7B3";
            Pin_Text.Text = new string('●', Pin_Text_Hidden.Text.Length);
        }
    }

    // Copy Pin Button (View Account Page)
    private void CopyPin_Button_Click(object _, RoutedEventArgs e)
    {
        var dataPackage = new DataPackage();
        dataPackage.SetText(Pin_Text_Hidden.Text);
        Clipboard.SetContent(dataPackage);
    }

    // Copy Password Button (View Account Page)
    private void CopyPassword_Button_Click(object _, RoutedEventArgs e)
    {
        var dataPackage = new DataPackage();
        dataPackage.SetText(Password_Text_Hidden.Text);
        Clipboard.SetContent(dataPackage);
    }

    // Copy Recovery Phone Number Button (View Account Page)
    private void CopyRecoveryPhoneNumber_Button_Click(object _, RoutedEventArgs e)
    {
        var dataPackage = new DataPackage();
        dataPackage.SetText(RecoveryPhoneNumber_Text.Text);
        Clipboard.SetContent(dataPackage);
    }

    // Copy Recovery Email Button (View Account Page)
    private void CopyRecoveryEmail_Button_Click(object _, RoutedEventArgs e)
    {
        var dataPackage = new DataPackage();
        dataPackage.SetText(RecoveryEmail_Text.Text);
        Clipboard.SetContent(dataPackage);
    }

    // Delete Account Button (View Account Page)
    private async void Delete_Button_Click(object _, RoutedEventArgs e)
    {
        var dialog = new ContentDialog
        {
            XamlRoot = XamlRoot,
            Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
            Title = "Confirm deletion?",
            PrimaryButtonText = "Delete",
            CloseButtonText = "Cancel",
            DefaultButton = ContentDialogButton.Primary,
            Content = "Are you sure you want to delete this record? This action cannot be undone and whole record will be permanently removed. Please confirm by typing 'DELETE' to proceed or 'CANCEL' to abort."
        };

        var result = await dialog.ShowAsync();

        if(result.ToString() == "Primary")
        {
            AccountDB.DeleteAccount(currentAccountId);
            AccountDL.DeleteAccount(currentAccountId);

            BackupCodeDL.DeleteBackupCodesByAccountId(currentAccountId);
            BackupCodeDB.DeleteBackupCodesByAccountId(currentAccountId);

            ViewAccount_Grid.Visibility = Visibility.Collapsed;
            ErrorContainer_Grid.Visibility = Visibility.Visible;

            RefreshAccountsListView();
        }

    }

    // Refresh Accounts List View
    public void RefreshAccountsListView()
    {
        AccountsListView.Children.Clear();
        radioButtons.Items.Clear();
    
        if (AccountDL.GetAccounts().Count == 0)
        {
            Accounts_ScrollViewer.Visibility = Visibility.Collapsed;
            NoAccounts_Grid.Visibility = Visibility.Visible;
        }
        else
        {        
            Accounts_ScrollViewer.Visibility = Visibility.Visible;
            NoAccounts_Grid.Visibility = Visibility.Collapsed;
        
            var accounts = AccountDL.GetAccounts();
        
            foreach (var account in accounts)
            {
                if (!string.IsNullOrEmpty(account.Email))
                {
                    AddAccountInListView(account.Id, account.Domain, account.Title, account.Email);
                }
                else if (!string.IsNullOrEmpty(account.Username))
                {
                    AddAccountInListView(account.Id, account.Domain, account.Title, account.Username);
                }
                else if (!string.IsNullOrEmpty(account.PhoneNumber))
                {
                    AddAccountInListView(account.Id, account.Domain, account.Title, account.PhoneNumber);
                }
                else
                {
                    AddAccountInListView(account.Id, account.Domain, account.Title, string.Empty);
                }
            }
        }
    }

    // Copy Authenticator Button (View Account Page)
    private void CopyOTP_Button_Click(object sender, RoutedEventArgs e)
    {
        var dataPackage = new DataPackage();
        dataPackage.SetText(Authenticator_Text.Text);
        Clipboard.SetContent(dataPackage);
    }

    // Show QR Code Button (View Account Page)
    private async void AuthenticatorQR_Button_Click(object sender, RoutedEventArgs e)
    {
        var scrollViewer = new ScrollViewer
        {
            Name = "AuthenticatorDialog_ScrollViewer",
            Width = 350,
            Height = 375
        };

        var dialog = new ContentDialog
        {
            XamlRoot = XamlRoot,
            Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
            Title = "Authenticator Qr Code",
            PrimaryButtonText = "Close",
            DefaultButton = ContentDialogButton.Primary,
            Content = scrollViewer
        };

        var stackPanel = new StackPanel
        {
            Name = "AuthenticatorDialog_StackPanel",
            Orientation = Orientation.Vertical,
            Margin = new Thickness(10, 1, 10, 1),
        };

        scrollViewer.Content = stackPanel;

        var qrCode_Image = new Image
        {
            Name = "AuthenticatorQR_Image",
            Width = 200,
            Height = 200,
            HorizontalAlignment = HorizontalAlignment.Center,
        };

        var account = AccountDL.GetAccountById(currentAccountId);
        var qrGenerator = new QRCodeGenerator();
        var qrCodeData = qrGenerator.CreateQrCode(account!.QrCode, QRCodeGenerator.ECCLevel.Q);
        var qrCode = new QRCode(qrCodeData);
        var qrCodeImage = qrCode.GetGraphic(10, "#ffffff", "#00000000");

        using var memory = new MemoryStream();
        qrCodeImage.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
        memory.Position = 0;
        var bitmapImage = new BitmapImage();
        await bitmapImage.SetSourceAsync(memory.AsRandomAccessStream());

        qrCode_Image.Source = bitmapImage;

        stackPanel.Children.Add(qrCode_Image);


        var copyQrCodeButton = new Button
        {
            Content = new FontIcon
            {
                Glyph = "\uE8C8",
                FontSize = 17
            },
            Margin = new Thickness(0, 0, 20, 0),
            BorderThickness = new Thickness(0),
            Height = 35,
            Width = 45,
            HorizontalAlignment = HorizontalAlignment.Center,
            Command = new RelayCommand(() =>
            {
                var dataPackage = new DataPackage();
                dataPackage.SetText(account!.QrCode);
                Clipboard.SetContent(dataPackage);
            })
        };

        ToolTipService.SetToolTip(copyQrCodeButton, "Copy QR Code");
        ToolTipService.SetPlacement(copyQrCodeButton, PlacementMode.Bottom);

        stackPanel.Children.Add(copyQrCodeButton);

        var authenticator = ParseUri(account.QrCode ?? "");

        if(authenticator != null)
        {
            if (!string.IsNullOrEmpty(authenticator.Type))
            {
                var TypeLabel = new TextBlock
                {
                    Text = "Auth Type",
                    Margin = new Thickness(0, 15, 0, 0),
                    Style = (Style)Application.Current.Resources["BodyStrongTextBlockStyle"],
                    FontSize = 17
                };

                var TypeText = new RichTextBlock
                {
                    Margin = new Thickness(0, 10, 0, 0),
                    FontSize = 13,
                    FontWeight = FontWeights.Bold,
                    Opacity = 0.9,
                    Blocks =
                    {
                        new Paragraph
                        {
                            Inlines =
                            {
                                new Run
                                {
                                    Text = authenticator.Type.ToUpper(),
                                }
                            }
                        }
                    }
                };

                stackPanel.Children.Add(TypeLabel);
                stackPanel.Children.Add(TypeText);
            }

            if (!string.IsNullOrEmpty(authenticator.Label))
            {
                var Label = new TextBlock
                {
                    Text = "Label",
                    Margin = new Thickness(0, 15, 0, 0),
                    Style = (Style)Application.Current.Resources["BodyStrongTextBlockStyle"],
                    FontSize = 17
                };

                var LabelText = new RichTextBlock
                {
                    Margin = new Thickness(0, 10, 0, 0),
                    FontSize = 13,
                    FontWeight = FontWeights.Bold,
                    Opacity = 0.9,
                    Blocks =
                    {
                        new Paragraph
                        {
                            Inlines =
                            {
                                new Run
                                {
                                    Text = authenticator.Label,
                                }
                            }
                        }
                    }
                };

                stackPanel.Children.Add(Label);
                stackPanel.Children.Add(LabelText);
            }

            if (!string.IsNullOrEmpty(authenticator.Issuer))
            {
                var IssuerLabel = new TextBlock
                {
                    Text = "Issuer",
                    Margin = new Thickness(0, 15, 0, 0),
                    Style = (Style)Application.Current.Resources["BodyStrongTextBlockStyle"],
                    FontSize = 17
                };

                var IssuerText = new RichTextBlock
                {
                    Margin = new Thickness(0, 10, 0, 0),
                    FontSize = 13,
                    FontWeight = FontWeights.Bold,
                    Opacity = 0.9,
                    Blocks =
                    {
                        new Paragraph
                        {
                            Inlines =
                            {
                                new Run
                                {
                                    Text = authenticator.Issuer,
                                }
                            }
                        }
                    }
                };

                stackPanel.Children.Add(IssuerLabel);
                stackPanel.Children.Add(IssuerText);
            }

            if (!string.IsNullOrEmpty(authenticator.Secret))
            {
                var SecretLabel = new TextBlock
                {
                    Text = "Secret",
                    Margin = new Thickness(0, 15, 0, 0),
                    Style = (Style)Application.Current.Resources["BodyStrongTextBlockStyle"],
                    FontSize = 17
                };

                var SecretText = new RichTextBlock
                {
                    Margin = new Thickness(0, 10, 0, 0),
                    FontSize = 13,
                    FontWeight = FontWeights.Bold,
                    Opacity = 0.9,
                    Blocks =
                    {
                        new Paragraph
                        {
                            Inlines =
                            {
                                new Run
                                {
                                    Text = authenticator.Secret.ToUpper(),
                                }
                            }
                        }
                    }
                };

                stackPanel.Children.Add(SecretLabel);
                stackPanel.Children.Add(SecretText);
            }

            if (!string.IsNullOrEmpty(authenticator.Algorithm))
            {
                var AlgorithmLabel = new TextBlock
                {
                    Text = "Algorithm",
                    Margin = new Thickness(0, 15, 0, 0),
                    Style = (Style)Application.Current.Resources["BodyStrongTextBlockStyle"],
                    FontSize = 17
                };

                var AlgorithmText = new RichTextBlock 
                { 
                    Margin = new Thickness(0, 10, 0, 0),
                    FontSize = 13,
                    FontWeight = FontWeights.Bold,
                    Opacity = 0.9,
                    Blocks = 
                    { 
                        new Paragraph 
                        { 
                            Inlines = 
                            { 
                                new Run 
                                { 
                                    Text = authenticator.Algorithm.ToUpper() 
                                } 
                            } 
                        } 
                    } 
                };

                stackPanel.Children.Add(AlgorithmLabel);
                stackPanel.Children.Add(AlgorithmText);
            }

            if (!string.IsNullOrEmpty(authenticator.Digits))
            {
                var DigitsLabel = new TextBlock
                {
                    Text = "Digits",
                    Margin = new Thickness(0, 15, 0, 0),
                    Style = (Style)Application.Current.Resources["BodyStrongTextBlockStyle"],
                    FontSize = 17
                };

                var DigitsText = new RichTextBlock
                {
                    Margin = new Thickness(0, 10, 0, 0),
                    FontSize = 13,
                    FontWeight = FontWeights.Bold,
                    Opacity = 0.9,
                    Blocks =
                    {
                        new Paragraph
                        {
                            Inlines =
                            {
                                new Run
                                {
                                    Text = authenticator.Digits,
                                }
                            }
                        }
                    }
                };

                stackPanel.Children.Add(DigitsLabel);
                stackPanel.Children.Add(DigitsText);
            }

            if (!string.IsNullOrEmpty(authenticator.Type))
            {
                if(authenticator.Type.ToUpper() == "TOTP")
                {
                    var PeriodLabel = new TextBlock
                    {
                        Text = "Period",
                        Margin = new Thickness(0, 15, 0, 0),
                        Style = (Style)Application.Current.Resources["BodyStrongTextBlockStyle"],
                        FontSize = 17
                    };

                    var PeriodText = new RichTextBlock
                    {
                        Margin = new Thickness(0, 10, 0, 0),
                        FontSize = 13,
                        FontWeight = FontWeights.Bold,
                        Opacity = 0.9,
                        Blocks =
                        {
                            new Paragraph
                            {
                                Inlines =
                                {
                                    new Run
                                    {
                                        Text = authenticator.Period,
                                    }
                                }
                            }
                        }
                    };

                    stackPanel.Children.Add(PeriodLabel);
                    stackPanel.Children.Add(PeriodText);
                }
                else if(authenticator.Type.ToUpper() == "HOTP")
                {
                    var CounterLabel = new TextBlock
                    {
                        Text = "Counter",
                        Margin = new Thickness(0, 15, 0, 0),
                        Style = (Style)Application.Current.Resources["BodyStrongTextBlockStyle"],
                        FontSize = 17
                    };

                    var CounterText = new RichTextBlock
                    {
                        Margin = new Thickness(0, 10, 0, 0),
                        FontSize = 13,
                        FontWeight = FontWeights.Bold,
                        Opacity = 0.9,
                        Blocks =
                        {
                            new Paragraph
                            {
                                Inlines =
                                {
                                    new Run
                                    {
                                        Text = authenticator.Counter,
                                    }
                                }
                            }
                        }
                    };

                    stackPanel.Children.Add(CounterLabel);
                    stackPanel.Children.Add(CounterText);
                }
            }
        }
     
        await dialog.ShowAsync();

    }

    // Add Backup Code Field (View Account Page)
    private void AddBackupCodeinContainer(string backupCodeValue, int isUsed, int accountId)
    {
        var grid = new Grid
        {
            Margin = new Thickness(0, 10, 0, 0),
        };

        // Create ColumnDefinitions for the Grid
        grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) }); // For Backup Code Text
        grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto }); // For Backup Code isUsed Checkbox & Backup Code Copy Button

        // Backup Code TextBlock
        var textBlock = new RichTextBlock
        {
            Opacity = 0.9,
            FontSize = 14,
            VerticalAlignment = VerticalAlignment.Center,
            FontWeight = Microsoft.UI.Text.FontWeights.Bold,
            Margin = new Thickness(0, 0, 0, 0),
            Blocks =
            {
                new Paragraph
                {
                    Inlines =
                    {
                        new Run
                        {
                            Text = backupCodeValue,
                        }
                    }
                }
            }
        };

        var stackPanel = new StackPanel
        {
            Orientation = Orientation.Horizontal
        };

        // Backup Code isUsed CheckBox
        var checkBox = new CheckBox
        {
            IsChecked = (isUsed == 1),
            IsEnabled = true,
            Padding = new Thickness(0),
            MinWidth = 0
        };

        ToolTipService.SetToolTip(checkBox, isUsed == 1 ? "Mark as Unused" : "Mark as Used");
        ToolTipService.SetPlacement(checkBox, PlacementMode.Bottom);

        checkBox.Checked += (sender, e) =>
        {
            // get the backup code object from account Id
            var updatedBackupCode = new BackupCode(accountId, backupCodeValue, 1);
            
            BackupCodeDL.UpdateBackupCode(updatedBackupCode);
            BackupCodeDB.UpdateBackupCodeStatus(accountId, backupCodeValue, 1);

            ToolTipService.SetToolTip(checkBox, "Mark as Unused");
        };

        checkBox.Unchecked += (sender, e) =>
        {
            // get the backup code object from account Id
            var updatedBackupCode = new BackupCode(accountId, backupCodeValue, 0);

            BackupCodeDL.UpdateBackupCode(updatedBackupCode);
            BackupCodeDB.UpdateBackupCodeStatus(accountId, backupCodeValue, 0);

            ToolTipService.SetToolTip(checkBox, "Mark as Used");
        };

        // Copy Backup Code Button
        var copyButton = new Button
        {
            Content = new FontIcon
            {           
                Glyph = "\uE8C8",
                FontSize = 17
            },
            Margin = new Thickness(15, 0, 20, 0),
            BorderThickness = new Thickness(0),
            Background = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["DesktopAcrylicTransparentBrush"],
            Height = 35,
            Width = 45,
            Command = new RelayCommand(() =>
            {      
                var dataPackage = new DataPackage();
                dataPackage.SetText(backupCodeValue);
                Clipboard.SetContent(dataPackage);
            })
        };

        // Set ToolTips for the Copy Backup Code Button
        ToolTipService.SetToolTip(copyButton, "Copy Backup Code");
        ToolTipService.SetPlacement(copyButton, PlacementMode.Bottom);

        stackPanel.Children.Add(checkBox);
        stackPanel.Children.Add(copyButton);

        // Set Grid.Column for each element
        Grid.SetColumn(textBlock, 0);
        Grid.SetColumn(stackPanel, 1);

        // Add the TextBlock, CheckBox, and Button to the Grid
        grid.Children.Add(textBlock);

        grid.Children.Add(stackPanel);



        BackupCodes_Grids.Children.Add(grid);

    }

    // Get Input Account (Add Account Page)
    private Account? GetInputAccount()
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
        var Notes = Notes_TextBox.Text;

        if (AccountType == "Custom")
        {
            if (Title == string.Empty || Domain == string.Empty)
            {
                MessageDialogHelper.ShowMessageDialog(XamlRoot, "Error", "Title and Domain are required fields.");
                return null;
            }
        }
        else
        {
            if (AccountType == "Google")
            {
                Domain = "google.com";
                Title = "Google";
            }
            else if (AccountType == "Microsoft")
            {
                Domain = "microsoft.com";
                Title = "Microsoft";
            }
            else if (AccountType == "Facebook")
            {
                Domain = "facebook.com";
                Title = "Facebook";
            }
            else if (AccountType == "Twitter")
            {
                Domain = "twitter.com";
                Title = "Twitter";
            }
            else if (AccountType == "Instagram")
            {
                Domain = "instagram.com";
                Title = "Instagram";
            }
            else if (AccountType == "LinkedIn")
            {
                Domain = "linkedin.com";
                Title = "LinkedIn";
            }
            else if (AccountType == "Snapchat")
            {
                Domain = "snapchat.com";
                Title = "Snapchat";
            }
            else
            {
                MessageDialogHelper.ShowMessageDialog(XamlRoot, "Error", "Please select an account type.");
                return null;
            }
        }

        Debug.WriteLine($"{Title}, {Domain}");

        if (Email == string.Empty && Username == string.Empty && PhoneNumber == string.Empty)
        {
            MessageDialogHelper.ShowMessageDialog(XamlRoot, "Error", "At least one of the following fields is required: \nEmail, Username, or Phone Number.");
            return null;
        }

        if (Password == string.Empty && Pin == string.Empty)
        {
            MessageDialogHelper.ShowMessageDialog(XamlRoot, "Error", "At least one of the following fields is required: \nPassword or Pin.");
            return null;
        }

        if(Email.Length > 0)
        {
            if (!Email.Contains('@') || Email.Contains(' ') || Email.Length < 5)
            {
                MessageDialogHelper.ShowMessageDialog(XamlRoot, "Error", "Invalid Email Address.");
                return null;
            }
        }

        if (Password.Length < 7 && Password.Length > 0)
        {
            MessageDialogHelper.ShowMessageDialog(XamlRoot, "Error", "Password must be at least 7 characters long.");
            return null;
        }

        var account = new Account
        (
            Id: AccountDB.GetMaxId() + 1,
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
            Notes: Notes ?? ""
        );

        return account;
    }

    // Get Input Backup Codes (Add Account Page)
    private List<BackupCode> GetInputBackupCodes()
    {
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
                    Code: backupCode.Text
                );
            
                backupCodes.Add(backupCodeBL);
            }
        }

        return backupCodes;
    }

    // Modify Account Button (View Account Page)
    private void Modify_Button_Click(object sender, RoutedEventArgs e)
    {
        ErrorContainer_Grid.Visibility = Visibility.Collapsed;
        ViewAccount_Grid.Visibility = Visibility.Collapsed;
        AddAccountContainer_Grid.Visibility = Visibility.Visible;

        Save_Button.Visibility = Visibility.Collapsed;
        Update_Button.Visibility = Visibility.Visible;

        var account = AccountDL.GetAccountById(currentAccountId);

        if (account == null)
        {        
            return;
        }

        AccountType_ComboBox.SelectedValue = account.Type;
        Title_TextBox.Text = account.Title;
        Domain_TextBox.Text = account.Domain;
        Name_TextBox.Text = account.Name;
        Email_TextBox.Text = account.Email;
        Username_TextBox.Text = account.Username;
        PhoneNumber_TextBox.Text = account.PhoneNumber;
        Password_TextBox.Password = account.Password;
        Pin_TextBox.Password = account.Pin;
        DateOfBirth_TextBox.Text = account.DateOfBirth;
        RecoveryEmail_TextBox.Text = account.RecoveryEmail;
        RecoveryPhoneNumber_TextBox.Text = account.RecoveryPhoneNumber;
        QrCode_TextBox.Text = account.QrCode;
        Notes_TextBox.Text = account.Notes;

        // backup codes
        var backupCodes = BackupCodeDL.GetBackupCodesByAccountId(currentAccountId);
        if (backupCodes != null)
        {
            var index = 0;
            if (backupCodes.Count > 0)
            {
                RemoveAllBackupCodesTextbox();
                foreach (var backupCode in backupCodes)
                {
                    if (index == 0)
                    {
                        BackupCode1_TextBox.Text = backupCode.Code;
                        index++;
                        continue;
                    }
                    else
                    {
                        AddBackupCode_Button_Click(null, null);
                        var backupCodeTextBox = (TextBox)BackupCodes_StackPanel.Children[index];
                        backupCodeTextBox.Text = backupCode.Code;
                    }

                }
            }
        }

        BackupCodes_Grids.Children.Clear();

    }

    // Update Account Button (Update(Add) Account Page)
    private void Update_Button_Click(object sender, RoutedEventArgs e)
    {
        ErrorContainer_Grid.Visibility = Visibility.Collapsed;
        AddAccountContainer_Grid.Visibility = Visibility.Collapsed;
        ViewAccount_Grid.Visibility = Visibility.Visible;

        var account = GetInputAccount();

        if (account == null)
        {        
            return;
        }

        account.Id = currentAccountId;

        AccountDL.UpdateAccount(account);
        AccountDB.UpdateAccount(account);

        var backupCodes = GetInputBackupCodes();
        backupCodes.ForEach(bc => bc.AccountId = account.Id);
        BackupCodeDL.DeleteBackupCodesByAccountId(account.Id ?? 0);
        backupCodes.ForEach(bc => BackupCodeDL.AddBackupCode(bc));
        BackupCodeDB.DeleteBackupCodesByAccountId(account.Id ?? 0);
        BackupCodeDB.StoreBackupCodes(backupCodes);

        RenderUserInterface(account);
    }

    // Apply Accounts Filter (Left Sidebar)
    private void ApplyFilter(string filterType)
    {    
        AccountsListView.Children.Clear();
        radioButtons.Items.Clear();
        currentAccountId = 0;
        currentAuthenticator = null; 
        backupCodeCount = 1;
        oldOTP = "";
        timer?.Dispose();
        ViewAccount_Grid.Visibility = Visibility.Collapsed;
        ErrorContainer_Grid.Visibility = Visibility.Visible;
        AddAccountContainer_Grid.Visibility = Visibility.Collapsed;

        var accounts = filterType switch
        {
            "Google" or "Microsoft" or "Facebook" or "Instagram" or "Twitter" or "Snapchat" or "LinkedIn" or "Custom" => AccountDL.GetAccountsByType(filterType),
            "Clear" => AccountDL.GetAccounts(),
            "Authenticator" => AccountDL.GetAccounts().Where(a => !string.IsNullOrEmpty(a.QrCode)).ToList(),
            "Newest_DateAdded" => AccountDL.GetAccounts().OrderByDescending(a => a.DateAdded).ToList(),
            "Oldest_DateAdded" => AccountDL.GetAccounts().OrderBy(a => a.DateAdded).ToList(),
            "Newest_DateModified" => AccountDL.GetAccounts().OrderByDescending(a => a.DateModified).ToList(),
            "Oldest_DateModified" => AccountDL.GetAccounts().OrderBy(a => a.DateModified).ToList(),
            _ => AccountDL.GetAccounts(),
        };

        if (accounts.Count == 0)
        {        
            Accounts_ScrollViewer.Visibility = Visibility.Collapsed;
            NoAccounts_Grid.Visibility = Visibility.Visible;
        }
        else
        {        
            Accounts_ScrollViewer.Visibility = Visibility.Visible;
            NoAccounts_Grid.Visibility = Visibility.Collapsed;
        }

        foreach (var account in accounts)
        {
            if (!string.IsNullOrEmpty(account.Email))
            {
                AddAccountInListView(account.Id, account.Domain, account.Title, account.Email);
            }
            else if (!string.IsNullOrEmpty(account.Username))
            {
                AddAccountInListView(account.Id, account.Domain, account.Title, account.Username);
            }
            else if (!string.IsNullOrEmpty(account.PhoneNumber))
            {
                AddAccountInListView(account.Id, account.Domain, account.Title, account.PhoneNumber);
            }
            else
            {
                AddAccountInListView(account.Id, account.Domain, account.Title, string.Empty);
            }
        }
    }

    // Google Filter Button (Left Sidebar)
    private void Google_Filter_Click(object sender, RoutedEventArgs e)
    {
        ApplyFilter("Google");
    }

    // Microsoft Filter Button (Left Sidebar)
    private void Microsoft_Filter_Click(object sender, RoutedEventArgs e)
    {
        ApplyFilter("Microsoft");
    }

    // Facebook Filter Button (Left Sidebar)
    private void Facebook_Filter_Click(object sender, RoutedEventArgs e)
    {
        ApplyFilter("Facebook");
    }

    // Instagram Filter Button (Left Sidebar)
    private void Instagram_Filter_Click(object sender, RoutedEventArgs e)
    {
        ApplyFilter("Instagram");
    }

    // Twitter Filter Button (Left Sidebar)
    private void Twitter_Filter_Click(object sender, RoutedEventArgs e)
    {
        ApplyFilter("Twitter");
    }

    // Snapchat Filter Button (Left Sidebar)
    private void Snapchat_Filter_Click(object sender, RoutedEventArgs e)
    {
        ApplyFilter("Snapchat");
    }

    // LinkedIn Filter Button (Left Sidebar)
    private void LinkedIn_Filter_Click(object sender, RoutedEventArgs e)
    {
        ApplyFilter("LinkedIn");
    }

    // Custom Filter Button (Left Sidebar)
    private void Custom_Filter_Click(object sender, RoutedEventArgs e)
    {
        ApplyFilter("Custom");
    }

    // Newest Date Added Filter Button (Left Sidebar)
    private void Newest_DateAdded_Filter_Click(object sender, RoutedEventArgs e)
    {
        ApplyFilter("Newest_DateAdded");
    }

    // Oldest Date Added Filter Button (Left Sidebar)
    private void Oldest_DateAdded_Filter_Click(object sender, RoutedEventArgs e)
    {
        ApplyFilter("Oldest_DateAdded");
    }

    // Newest Date Modified Filter Button (Left Sidebar)
    private void Newest_DateModified_Filter_Click(object sender, RoutedEventArgs e)
    {
        ApplyFilter("Newest_DateModified");
    }

    // Oldest Date Modified Filter Button (Left Sidebar)
    private void Oldest_DateModified_Filter_Click(object sender, RoutedEventArgs e)
    {
        ApplyFilter("Oldest_DateModified");
    }

    // Authenticator Filter Button (Left Sidebar)
    private void Authenticator_Filter_Click(object sender, RoutedEventArgs e)
    {
        ApplyFilter("Authenticator");
    }

    // Clear Filter Button (Left Sidebar)
    private void Clear_Filter_Click(object sender, RoutedEventArgs e)
    {
        ApplyFilter("Clear");
    }

    // Search Bar KeyDown Event
    private void SearchBar_KeyDown(object sender, KeyRoutedEventArgs e)
    {
        if(SearchBar.Text == string.Empty)
        {
            RefreshAccountsListView();
        }
        if (e.Key == Windows.System.VirtualKey.Enter)
        {
            var searchQuery = SearchBar.Text;

            if (searchQuery != string.Empty)
            {
                if(searchQuery.Length < 3)
                {                
                    MessageDialogHelper.ShowMessageDialog(XamlRoot, "Error", "Search query must be at least 3 characters long.");
                    return;
                }

                var accounts = AccountDL.GetAccounts()
                    .Where(a =>
                        (a.Domain != null && a.Domain.ToLower().Contains(searchQuery.ToLower())) ||
                        (a.Title != null && a.Title.ToLower().Contains(searchQuery.ToLower())) ||
                        (!string.IsNullOrEmpty(a.Email) && a.Email.ToLower().Contains(searchQuery.ToLower())) ||
                        (!string.IsNullOrEmpty(a.Username) && a.Username.ToLower().Contains(searchQuery.ToLower())) ||
                        (!string.IsNullOrEmpty(a.PhoneNumber) && a.PhoneNumber.ToLower().Contains(searchQuery.ToLower()))
                )
                .ToList();

                if (accounts.Count > 0)
                {
                    AccountsListView.Children.Clear();
                    radioButtons.Items.Clear();
                    currentAccountId = 0;
                    currentAuthenticator = null;
                    backupCodeCount = 1;
                    oldOTP = "";
                    timer?.Dispose();
                    ViewAccount_Grid.Visibility = Visibility.Collapsed;
                    ErrorContainer_Grid.Visibility = Visibility.Visible;
                    AddAccountContainer_Grid.Visibility = Visibility.Collapsed;
                
                    foreach (var account in accounts)
                    {
                        if (!string.IsNullOrEmpty(account.Email))
                        {
                            AddAccountInListView(account.Id, account.Domain, account.Title, account.Email);
                        }
                        else if (!string.IsNullOrEmpty(account.Username))
                        {
                            AddAccountInListView(account.Id, account.Domain, account.Title, account.Username);
                        }
                        else if (!string.IsNullOrEmpty(account.PhoneNumber))
                        {
                            AddAccountInListView(account.Id, account.Domain, account.Title, account.PhoneNumber);
                        }
                        else
                        {
                            AddAccountInListView(account.Id, account.Domain, account.Title, string.Empty);
                        }
                    }
                }
                else
                {
                    MessageDialogHelper.ShowMessageDialog(XamlRoot, "No Results", "No accounts found matching the search query.");
                }
            }
        }
    }


    // Authenticator Configure Button (Add Account Page)
    private async void AuthenticatorConfigure_Button_Click(object sender, RoutedEventArgs e)
    {
        var Container_ScrollViewer = new ScrollViewer
        {        
            Name = "Container_ScrollViewer",
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
            Margin = new Thickness(0, 0, 0, 10),
            Width = 400,
            Height = 300
        };

        var Container_StackPanel = new StackPanel
        {
            Orientation = Orientation.Vertical,
            Margin = new Thickness(10, 1, 10, 1)
        };

        var Type_ComboBox = new ComboBox
        {        
            Name = "Type_ComboBox",
            Header = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Children =
                {
                    new TextBlock
                    {
                        Text = "Type",
                        Margin = new Thickness(0, 0, 5, 0)
                    },
                    new FontIcon
                    {
                        Glyph = "\uE735",
                        FontSize = 13,
                    }
                }
            },
            Margin = new Thickness(0, 0, 0, 15),
            PlaceholderText = "Select Type",
            Items = { "TOTP", "HOTP" },
            SelectedIndex = 0,
            Width = 200
        };

        var Label_TextBox = new TextBox
        {
            Name = "Label_TextBox",
            Header = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Children =
                {
                    new TextBlock
                    {
                        Text = "Label",
                        Margin = new Thickness(0, 0, 5, 0)
                    },
                    new FontIcon
                    {
                        Glyph = "\uE734",
                        FontSize = 13,
                    }
                }
            },
            Margin = new Thickness(0, 0, 0, 15),
            PlaceholderText = "affan@gmail.com",
        };

        var Issuer_TextBox = new TextBox
        {        
            Name = "Issuer_TextBox",
            Header = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Children =
                {
                    new TextBlock
                    {
                        Text = "Issuer",
                        Margin = new Thickness(0, 0, 5, 0)
                    },
                    new FontIcon
                    {
                        Glyph = "\uE734",
                        FontSize = 13,
                    }
                }
            },
            Margin = new Thickness(0, 0, 0, 15),
            PlaceholderText = "Google",
        };

        var SecretKey_TextBox = new TextBox
        {        
            Name = "SecretKey_TextBox",
            Header = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Children =
                {
                    new TextBlock
                    {
                        Text = "Secret Key",
                        Margin = new Thickness(0, 0, 5, 0)
                    },
                    new FontIcon
                    {
                        Glyph = "\uE735",
                        FontSize = 13,
                    }
                }
            },
            Margin = new Thickness(0, 0, 0, 15),
            PlaceholderText = "JBSWY3DPEHPK3PXPLA",
        };

        var Digits_TextBox = new TextBox
        {               
            Name = "Digits_TextBox",
            Header = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Children =
                {
                    new TextBlock
                    {
                        Text = "Digits",
                        Margin = new Thickness(0, 0, 5, 0)
                    },
                    new FontIcon
                    {
                        Glyph = "\uE734",
                        FontSize = 13,
                    }
                }
            },
            Margin = new Thickness(0, 0, 0, 15),
            PlaceholderText = "6",
        };

        var Algorithm_ComboBox = new ComboBox
        {               
            Name = "Algorithm_ComboBox",
            Header = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Children =
                {
                    new TextBlock
                    {
                        Text = "Algorithm",
                        Margin = new Thickness(0, 0, 5, 0)
                    },
                    new FontIcon
                    {
                        Glyph = "\uE734",
                        FontSize = 13,
                    }
                }
            },
            Margin = new Thickness(0, 0, 0, 15),
            PlaceholderText = "Select Algorithm",
            Items = { "SHA1", "SHA256", "SHA512" },
            SelectedIndex = 0,
            Width = 200
        };

        var Period_TextBox = new TextBox
        {                      
            Name = "Period_TextBox",
            Header = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Children =
                {
                    new TextBlock
                    {
                        Text = "Period",
                        Margin = new Thickness(0, 0, 5, 0)
                    },
                    new FontIcon
                    {
                        Glyph = "\uE734",
                        FontSize = 13,
                    }
                }
            },
            Margin = new Thickness(0, 0, 0, 15),
            PlaceholderText = "30",
        };

        var Counter_TextBox = new TextBox
        {                             
            Name = "Counter_TextBox",
            Header = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Children =
                {
                    new TextBlock
                    {
                        Text = "Counter",
                        Margin = new Thickness(0, 0, 5, 0)
                    },
                    new FontIcon
                    {
                        Glyph = "\uE734",
                        FontSize = 13,
                    }
                }
            },
            Margin = new Thickness(0, 0, 0, 15),
            PlaceholderText = "0",
            Visibility = Visibility.Collapsed
        };

        Container_StackPanel.Children.Add(Type_ComboBox);
        Container_StackPanel.Children.Add(Label_TextBox);
        Container_StackPanel.Children.Add(Issuer_TextBox);
        Container_StackPanel.Children.Add(SecretKey_TextBox);
        Container_StackPanel.Children.Add(Digits_TextBox);
        Container_StackPanel.Children.Add(Algorithm_ComboBox);
        Container_StackPanel.Children.Add(Period_TextBox);
        Container_StackPanel.Children.Add(Counter_TextBox);
        Container_ScrollViewer.Content = Container_StackPanel;

        Type_ComboBox.SelectionChanged += (sender, e) =>
        {
            var selectedType = Type_ComboBox.SelectedValue.ToString();
            if (selectedType == "TOTP")
            {
                Period_TextBox.Visibility = Visibility.Visible;
                Counter_TextBox.Visibility = Visibility.Collapsed;
            }
            else
            {
                Period_TextBox.Visibility = Visibility.Collapsed;
                Counter_TextBox.Visibility = Visibility.Visible;
            }
        };

        var authenticator = ParseUri(QrCode_TextBox.Text);
        if (authenticator != null)
        {
            if(authenticator.Type!.ToUpper() == "TOTP")
            {            
                Type_ComboBox.SelectedValue = "TOTP";
                Period_TextBox.Text = authenticator.Period;
            }
            else
            {            
                Type_ComboBox.SelectedValue = "HOTP";
                Counter_TextBox.Text = authenticator.Counter;
            }

            Label_TextBox.Text = authenticator.Label;
            Issuer_TextBox.Text = authenticator.Issuer;
            SecretKey_TextBox.Text = authenticator.Secret;
            Digits_TextBox.Text = authenticator.Digits;
            Algorithm_ComboBox.SelectedValue = authenticator.Algorithm!.ToUpper();
        }


        var dialog = new ContentDialog
        {
            XamlRoot = XamlRoot,
            Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style,
            Title = "Configure Authenticator",
            PrimaryButtonText = "Generate QR Code Link",
            CloseButtonText = "Cancel",
            DefaultButton = ContentDialogButton.Primary,
            Content = Container_ScrollViewer
        };

        var result = await dialog.ShowAsync();

        if (result.ToString() == "Primary")
        {
            if (string.IsNullOrEmpty(SecretKey_TextBox.Text))
            {
                return;
            }

            // Parse digits
            var digits = 0;
            if (!int.TryParse(Digits_TextBox.Text, out digits))
            {
                digits = 6; // Default value
            }

            // Parse period (only for TOTP)
            var period = 0;
            if (Type_ComboBox.SelectedValue.ToString() == "TOTP" && !int.TryParse(Period_TextBox.Text, out period))
            {
                period = 30; // Default value
            }

            // Parse counter (only for HOTP)
            var counter = 0;
            if (Type_ComboBox.SelectedValue.ToString() == "HOTP" && !int.TryParse(Counter_TextBox.Text, out counter))
            {
                counter = 0; // Default value
            }

            var uriString = new OtpUri(
                schema: Type_ComboBox.SelectedValue.ToString() == "TOTP" ? OtpType.Totp : OtpType.Hotp,
                secret: SecretKey_TextBox.Text,
                user: Label_TextBox.Text,
                issuer: Issuer_TextBox.Text,
                algorithm: Algorithm_ComboBox.SelectedValue.ToString() == "SHA1" ? OtpHashMode.Sha1 : Algorithm_ComboBox.SelectedValue.ToString() == "SHA256" ? OtpHashMode.Sha256 : OtpHashMode.Sha512,
                digits: digits,
                period: Type_ComboBox.SelectedValue.ToString() == "TOTP" ? period : 30,
                counter: Type_ComboBox.SelectedValue.ToString() == "HOTP" ? counter : 0
            );

            QrCode_TextBox.Text = uriString.ToString();
        }
    }

    private static Authenticator? ParseUri(string url)
    {
        if (string.IsNullOrEmpty(url))
        {
            return null;
        }

        var uri = new Uri(url);

        var scheme = uri.Scheme.ToLower();
        if (scheme != "otpauth")
        {
            return null;
        }

        var type = uri.Host.ToLower();
        if(type != "totp" && type != "hotp")
        {        
            return null;
        }

        var query = System.Web.HttpUtility.ParseQueryString(uri.Query);

        if (string.IsNullOrEmpty(query.Get("secret")))
        {
            return null;
        }

        string? label;
        try
        {
            var parts = uri.AbsolutePath.Trim('/').Split(':');
            label = Uri.UnescapeDataString(parts[1]);
        }
        catch (IndexOutOfRangeException)
        {
            label = "";
        }

        var authenticator = new Authenticator
        {
            Type = type,
            Label = label,
            Secret = query.Get("secret"),
            Issuer = query.Get("issuer"),
            Algorithm = query.Get("algorithm"),
            Digits = query.Get("digits"),
            Period = query.Get("period"),
            Counter = query.Get("counter")
        };

        return authenticator;
    }

    private void HOTP_Generate_Button_Click(object sender, RoutedEventArgs e)
    {
        var authenticator = currentAuthenticator;

        if (authenticator != null)
        {
            if (authenticator.Type!.ToUpper() == "HOTP")
            {
                var counter = authenticator.Counter;
                if (string.IsNullOrEmpty(counter))
                {
                    counter = "0";
                }

                var algo = authenticator.Algorithm;
                if (string.IsNullOrEmpty(algo))
                {
                    algo = "SHA1";
                }
                algo = algo.ToUpper();

                var digits = authenticator.Digits;
                if (string.IsNullOrEmpty(digits))
                {
                    digits = "6";
                }

                var obj = new Hotp(
                    secretKey: Base32Encoding.ToBytes(authenticator.Secret),
                    mode: algo == "SHA1" ? OtpHashMode.Sha1 : algo == "SHA256" ? OtpHashMode.Sha256 : OtpHashMode.Sha512,
                    hotpSize: int.Parse(digits)
                );

                var otp = obj.ComputeHOTP(long.Parse(counter));
                Authenticator_Text.Text = otp;
                currentAuthenticator!.Counter = (int.Parse(counter) + 1).ToString();
            }
        }
        
    }

}
