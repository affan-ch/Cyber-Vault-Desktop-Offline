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
using static QRCoder.PayloadGenerator;


namespace Cyber_Vault.Views;

public sealed partial class AccountsPage : Page
{
    private int currentAccountId = 0;
    private int backupCodeCount = 1;
    private Timer? timer;
    private string currentSecretKey = "";
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

    private void AddAccountInListView(int id, string url, string title, string email)
    {

        // Create a new StackPanel dynamically
        var dynamicStackPanel = new Grid
        {
            Margin = new Thickness(0, 1, 20, 5),
            Height = 70,
            Background = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["DesktopAcrylicTransparentBrush"],
            CornerRadius = new CornerRadius(5),
            
        };

        // Create ColumnDefinitions for the Grid
        dynamicStackPanel.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto }); // For image
        dynamicStackPanel.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) }); // For inner stack panel
        dynamicStackPanel.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto }); // For spacer
        dynamicStackPanel.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto }); // For font icon


        dynamicStackPanel.PointerEntered += (sender, e) =>
        {
            dynamicStackPanel.Background = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["LayerOnAcrylicFillColorDefaultBrush"];
        };

        dynamicStackPanel.PointerExited += (sender, e) =>
        {
             dynamicStackPanel.Background = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["DesktopAcrylicTransparentBrush"];
        };

        var radioButton = new RadioButton
        {
            Name = id.ToString(),
            IsChecked = false,
            Visibility = Visibility.Collapsed,
        };

        radioButton.Unchecked += (sender, e) =>
        {
            dynamicStackPanel.Background = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["DesktopAcrylicTransparentBrush"];
            
            dynamicStackPanel.PointerEntered += (sender, e) =>
            {
                dynamicStackPanel.Background = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["LayerOnAcrylicFillColorDefaultBrush"];
            };

            dynamicStackPanel.PointerExited += (sender, e) =>
            {
                dynamicStackPanel.Background = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["DesktopAcrylicTransparentBrush"];
            };
        };

        radioButtons.Items.Add(radioButton);

        // Create an Image and set its properties
        var image = new Microsoft.UI.Xaml.Controls.Image
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
        dynamicStackPanel.Children.Add(image);
        dynamicStackPanel.Children.Add(innerStackPanel);
        dynamicStackPanel.Children.Add(fontIcon);

        dynamicStackPanel.PointerPressed += (sender, e) =>
        {
            foreach (var rb in radioButtons.Items.Cast<RadioButton>())
            {
                if (rb.Name == id.ToString())
                {
                    rb.IsChecked = true;
                    Debug.WriteLine(rb.Name);
                    currentAccountId = id;
                    OTP_Ring.Value = 100;
                    timer?.Dispose();
                    dynamicStackPanel.Background = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["LayerOnAcrylicFillColorDefaultBrush"];
                    dynamicStackPanel.PointerEntered += (sender, e) =>
                    {
                        dynamicStackPanel.Background = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["LayerOnAcrylicFillColorDefaultBrush"];
                    };
                    dynamicStackPanel.PointerExited += (sender, e) =>
                    {                    
                        dynamicStackPanel.Background = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["LayerOnAcrylicFillColorDefaultBrush"];
                    };

                    ErrorContainer_Grid.Visibility = Visibility.Collapsed;
                    AddAccountContainer_Grid.Visibility = Visibility.Collapsed;
                    ViewAccount_Grid.Visibility = Visibility.Visible;

                    var account = AccountDL.GetAccountById(id);

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

        AccountsListView.Children.Add(dynamicStackPanel);
    }
    
    private string oldOTP = "";

    // Method to update OTP (View Account Page - Authenticator)
    private void UpdateOTP(string secret)
    {
        var totp = new Totp(Base32Encoding.ToBytes(secret), step: 30, mode: OtpHashMode.Sha1, totpSize: 6, timeCorrection: TimeCorrection.UncorrectedInstance);
        var otp = totp.ComputeTotp(DateTime.UtcNow);
        var remainingSeconds = totp.RemainingSeconds(DateTime.UtcNow);

        if (oldOTP == "")
        {
            oldOTP = otp;
            SetProgressRingValue(remainingSeconds);
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
            SetProgressRingValue(remainingSeconds);
        }
        else if (oldOTP == otp)
        {
            SetProgressRingValue(remainingSeconds);
        }

        
    }

    // Method to set the progress ring value (View Account Page)
    private void SetProgressRingValue(int remainingSeconds)
    {
        // Calculate the progress ring value based on remaining seconds
        var progressValue = (remainingSeconds / 30.0) * 100.0;

        // Set the progress ring value
        DispatcherQueue.TryEnqueue(() => {
            OTP_Ring.Value = progressValue;
        });
    }

    // Timer callback method (View Account Page - Authenticator Timer)
    private void TimerCallback(object? state)
    {
        if(currentSecretKey != null && currentSecretKey != "")
        {
            UpdateOTP(currentSecretKey);
        }
    }


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
        if ((account.QrCode != null && account.QrCode != string.Empty) || (account.Secret != null && account.Secret != string.Empty))
        {
            Authenticator_Container.Visibility = Visibility.Visible;

            if (account.Secret != null)
            {
                currentSecretKey = account.Secret;

                // Timer to update the OTP and ProgressRing value
                timer = new Timer(TimerCallback, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(300));
            }
        }
        else
        {
            currentSecretKey = "";
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
                AddBackupCodeinContainer(backupCode.Code ?? "", backupCode.IsUsed ?? 0);
            }
        }
        else
        {
            BackupCodes_Container.Visibility = Visibility.Collapsed;
        }

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

    private void Save_Button_Click(object sender, RoutedEventArgs e)
    {
        var account = GetInputAccount();

        if(account == null)
        {        
            return;
        }

        AccountDL.AddAccount(account);
        AccountDB.StoreAccount(account);
        AddAccountInListView(account.Id ?? 0, $"https://www.google.com/s2/favicons?domain={account.Domain}&sz=128", account.Title ?? "Custom Account", account.Email ?? "");

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
        SecretKey_TextBox.Text = string.Empty;
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
                AddAccountInListView(account.Id ?? 0, $"https://www.google.com/s2/favicons?domain={account.Domain}&sz=128", account.Title!, account.Email!);
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




        var issuerIndex = account.QrCode!.IndexOf("&issuer=");

        if (issuerIndex != -1)
        {
            // Extract the substring containing the issuer parameter
            var issuerSubstring = account.QrCode!.Substring(issuerIndex + "&issuer=".Length);

            // Find the index of the next parameter delimiter
            var nextDelimiterIndex = issuerSubstring.IndexOf('&');

            // Extract the issuer value
            var issuerValue = nextDelimiterIndex != -1 ? issuerSubstring[..nextDelimiterIndex] : issuerSubstring;

            var issuer = Uri.UnescapeDataString(issuerValue);

            var IssuerLabel = new TextBlock
            {
                Text = "Issuer",
                Margin = new Thickness(0, 15, 0, 0),
                Style = (Style)Application.Current.Resources["BodyStrongTextBlockStyle"],
                FontSize = 17
            };

            var IssuerText = new TextBlock
            {
                Text = issuer,
                Margin = new Thickness(0, 10, 0, 0),
                FontSize = 13,
                FontWeight = FontWeights.Bold,
                Opacity = 0.9
            };

            stackPanel.Children.Add(IssuerLabel);
            stackPanel.Children.Add(IssuerText);
        }


        var SecretLabel = new TextBlock
        {
            Text = "Secret",
            Margin = new Thickness(0, 15, 0, 0),
            Style = (Style)Application.Current.Resources["BodyStrongTextBlockStyle"],
            FontSize = 17
        };

        var SecretText = new TextBlock
        {
            Text = account.Secret?.ToUpper(),
            Margin = new Thickness(0,10,0,0),
            FontSize = 13,
            FontWeight = FontWeights.Bold,
            Opacity = 0.9
        };

        stackPanel.Children.Add(SecretLabel);
        stackPanel.Children.Add(SecretText);


        await dialog.ShowAsync();

    }

    // Add Backup Code Field (View Account Page)
    private void AddBackupCodeinContainer(string backupCode, int isUsed)
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
                            Text = backupCode,
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
                dataPackage.SetText(backupCode);
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
        var SecretKey = SecretKey_TextBox.Text;
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
            else if (AccountType == "GitHub")
            {
                Domain = "github.com";
                Title = "GitHub";
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

        if (!Email.Contains('@') || Email.Contains(' ') || Email.Length < 5)
        {
            MessageDialogHelper.ShowMessageDialog(XamlRoot, "Error", "Invalid Email Address.");
            return null;
        }

        if (Password.Length < 7)
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
            Secret: SecretKey ?? "",
            Notes: Notes ?? ""
        );

        return account;
    }

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
        SecretKey_TextBox.Text = account.Secret;
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

    // Update Account Button (Update Account Page)
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


}
