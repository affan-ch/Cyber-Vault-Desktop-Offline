using Cyber_Vault.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Controls.AnimatedVisuals;
using Cyber_Vault.BL;
using Cyber_Vault.DL;
using Cyber_Vault.DB;
using Cyber_Vault.Utils;
using Microsoft.UI.Xaml.Media.Imaging;
using System.Diagnostics;
using Microsoft.UI.Xaml.Input;
using Windows.ApplicationModel.DataTransfer;

namespace Cyber_Vault.Views;

public sealed partial class CreditCardsPage : Page
{
    private int currentcreditcardId = 0;
    private readonly RadioButtons radioButtons = new()
    {
        Visibility = Visibility.Collapsed
    };

    public CreditCardsViewModel ViewModel
    {
        get;
    }

    public CreditCardsPage()
    {
        ViewModel = App.GetService<CreditCardsViewModel>();
        InitializeComponent();
        RefreshCreditCardsListView();

    }

    // Refresh Credit Cards List View
    public void RefreshCreditCardsListView()
    {
        CreditCardsListView.Children.Clear();
        radioButtons.Items.Clear();

        if (CreditCardDL.GetCreditCards().Count == 0)
        {
            CreditCards_ScrollViewer.Visibility = Visibility.Collapsed;
            NoCreditCards_Grid.Visibility = Visibility.Visible;
        }
        else
        {
            CreditCards_ScrollViewer.Visibility = Visibility.Visible;
            NoCreditCards_Grid.Visibility = Visibility.Collapsed;

            var creditCards = CreditCardDL.GetCreditCards();

            foreach (var card in creditCards)
            {
                
                AddCreditCardInListView(card.Id, card.CardIssuer, card.CardNumber);
                
            }
        }
    }

    // Add Credit Cards In List View
    private void AddCreditCardInListView(int? id, string? title, string? subtitle)
    {
        NoCreditCards_Grid.Visibility = Visibility.Collapsed;
        CreditCardsListView.Visibility = Visibility.Visible;
        // Create a new StackPanel dynamically
        var creditcardContainer = new Grid
        {
            Margin = new Thickness(0, 1, 20, 5),
            Height = 70,
            Background = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["DesktopAcrylicTransparentBrush"],
            CornerRadius = new CornerRadius(5),
        };

        // Create ColumnDefinitions for the Grid
        creditcardContainer.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto }); // For image
        creditcardContainer.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) }); // For inner stack panel
        creditcardContainer.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto }); // For spacer
        creditcardContainer.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto }); // For font icon


        creditcardContainer.PointerEntered += (sender, e) =>
        {
            creditcardContainer.Background = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["LayerOnAcrylicFillColorDefaultBrush"];
        };

        creditcardContainer.PointerExited += (sender, e) =>
        {
            creditcardContainer.Background = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["DesktopAcrylicTransparentBrush"];
        };

        var radioButton = new RadioButton
        {
            Name = id.ToString(),
            IsChecked = false,
            Visibility = Visibility.Collapsed,
        };

        radioButton.Unchecked += (sender, e) =>
        {
            creditcardContainer.Background = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["DesktopAcrylicTransparentBrush"];

            creditcardContainer.PointerEntered += (sender, e) =>
            {
                creditcardContainer.Background = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["LayerOnAcrylicFillColorDefaultBrush"];
            };

            creditcardContainer.PointerExited += (sender, e) =>
            {
                creditcardContainer.Background = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["DesktopAcrylicTransparentBrush"];
            };
        };

        radioButtons.Items.Add(radioButton);


        var image = new Image
        {
            Margin = new Thickness(12, 10, 10, 10),
            Source = new BitmapImage(new Uri("ms-appx:///Assets/card.png")),
            Width = 35,
            Height = 35
        };

        // Create the inner StackPanel
        var innerStackPanel = new StackPanel
        {
            Orientation = Orientation.Vertical,
            Margin = new Thickness(5, 15, 0, 0)
        };

        // Card Issuer TextBlock
        var textBlock1 = new TextBlock
        {
            Text = title ?? "Custom",
            Style = (Style)Application.Current.Resources["BaseTextBlockStyle"],
            FontSize = 17
        };

        // Card Number TextBlock
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
        creditcardContainer.Children.Add(image);
        creditcardContainer.Children.Add(innerStackPanel);
        creditcardContainer.Children.Add(fontIcon);

        creditcardContainer.PointerPressed += (sender, e) =>
        {
            foreach (var rb in radioButtons.Items.Cast<RadioButton>())
            {
                if (rb.Name == id.ToString())
                {
                    rb.IsChecked = true;
                    Debug.WriteLine(rb.Name);
                    creditcardContainer.Background = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["LayerOnAcrylicFillColorDefaultBrush"];
                    creditcardContainer.PointerEntered += (sender, e) =>
                    {
                        creditcardContainer.Background = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["LayerOnAcrylicFillColorDefaultBrush"];
                    };
                    creditcardContainer.PointerExited += (sender, e) =>
                    {
                        creditcardContainer.Background = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["LayerOnAcrylicFillColorDefaultBrush"];
                    };

                    ErrorContainer_Grid.Visibility = Visibility.Collapsed;
                    AddCreditCardContainer_Grid.Visibility = Visibility.Collapsed;
                    ViewCreditCard_Grid.Visibility = Visibility.Visible;
                    currentcreditcardId = id ?? 0;

                    var creditcard = CreditCardDL.GetCreditCardById(id ?? 0);

                    if (creditcard == null)
                    {
                        return;
                    }

                    RenderUserInterface(creditcard);
                }
                else
                {
                    rb.IsChecked = false;
                }
            }
        };

        CreditCardsListView.Children.Add(creditcardContainer);
    }

    // Add Credit Card Button
    private void AddCreditCard_Button_Click(object sender, RoutedEventArgs e)
    {
        AddCreditCardContainer_Grid.Visibility = Visibility.Visible;
        ClearFields();
        Save_Button.Visibility = Visibility.Visible;
        ErrorContainer_Grid.Visibility = Visibility.Collapsed;
        ViewCreditCard_Grid.Visibility = Visibility.Collapsed;
        Update_Button.Visibility = Visibility.Collapsed;

        foreach (var rb in radioButtons.Items.Cast<RadioButton>())
        {
            rb.IsChecked = false;
        }
    }
    // Saving All the Data
    private void Save_Button_Click(object sender, RoutedEventArgs e)
    {
        var creditcard = GetInputCard();
        if (creditcard == null)
        {
            return;
        }

        AddCreditCardInListView(creditcard.Id, creditcard.CardIssuer, creditcard.CardNumber);
        

        CreditCardDL.AddCreditCard(creditcard);
        CreditCardDB.StoreCreditCard(creditcard);
        ClearFields();
        MessageDialogHelper.ShowMessageDialog(XamlRoot, "Success", "Credit Card added successfully!");
    }

    // Fill all the data in the fields
    private void RenderUserInterface(CreditCard creditcard)
    {

        // Card Holder Name
        if (creditcard.CardHolderName != null && creditcard.CardHolderName != string.Empty)
        {
            CardHolderName_Container.Visibility = Visibility.Visible;
            CardHolderName_Text.Text = creditcard.CardHolderName;
        }
        else
        {
            CardHolderName_Container.Visibility = Visibility.Collapsed;
        }

        // Card Number
        if (creditcard.CardNumber != null && creditcard.CardNumber != string.Empty)
        {
            CardNumber_Container.Visibility = Visibility.Visible;
            CardNumber_Text.Text = new string('●', creditcard.CardNumber.Length);
            CardNumber_Hidden.Text = creditcard.CardNumber;
            ToggleCardNumber_CheckBox.IsChecked = false;
            CardNumberToggle_Icon.Glyph = "\uE7B3";
            ToolTipService.SetToolTip(ToggleCardNumber_Button, "Show Card Number");
        }
        else
        {
            CardNumber_Container.Visibility = Visibility.Collapsed;
        }

        // Expiry Month
        if (creditcard.ExpiryMonth != null && creditcard.ExpiryMonth != string.Empty)
        {
            ExpiryMonth_Container.Visibility = Visibility.Visible;
            ExpiryMonth_Text.Text = creditcard.ExpiryMonth;
        }
        else
        {
            ExpiryMonth_Container.Visibility = Visibility.Collapsed;
        }

        // Expiry Year
        if (creditcard.ExpiryYear != null && creditcard.ExpiryYear != string.Empty)
        {
            ExpiryYear_Container.Visibility = Visibility.Visible;
            ExpiryYear_Text.Text = creditcard.ExpiryYear;
        }
        else
        {
            ExpiryYear_Container.Visibility = Visibility.Collapsed;
        }

        // CVV
        if (creditcard.CVV != null && creditcard.CVV != string.Empty)
        {
            CVV_Container.Visibility = Visibility.Visible;
            CVV_Text.Text = new string('●', creditcard.CVV.Length);
            CVV_Hidden.Text = creditcard.CVV;
            ToggleCVV_CheckBox.IsChecked = false;
            CVVToggle_Icon.Glyph = "\uE7B3";
            ToolTipService.SetToolTip(ToggleCVV_Button, "Show CVV");

        }
        else
        {
            CVV_Container.Visibility = Visibility.Collapsed;
        }

        // Card Pin
        if (creditcard.Pin != null && creditcard.Pin != string.Empty)
        {
            CardPin_Container.Visibility = Visibility.Visible;
            CardPin_Text.Text = new string('●', creditcard.Pin.Length);
            CardPin_Hidden.Text = creditcard.Pin;
            ToggleCardPin_CheckBox.IsChecked = false;
            CardPinToggle_Icon.Glyph = "\uE7B3";
            ToolTipService.SetToolTip(ToggleCardPin_Button, "Show Card Pin");
        }
        else
        {
            CardPin_Container.Visibility = Visibility.Collapsed;
        }

        // Card Issuer
        if (creditcard.CardIssuer != null && creditcard.CardIssuer != string.Empty)
        {
            CardIssuer_Container.Visibility = Visibility.Visible;
            CardIssuer_Text.Text = creditcard.CardIssuer;
        }
        else
        {
            CardIssuer_Container.Visibility = Visibility.Collapsed;
        }

        // Card Type
        if (creditcard.CardType != null && creditcard.CardType != string.Empty)
        {
            CardType_Container.Visibility = Visibility.Visible;
            CardType_Text.Text = creditcard.CardType;
        }
        else
        {
            CardType_Container.Visibility = Visibility.Collapsed;
        }

        // Billing Address
        if (creditcard.BillingAddress != null && creditcard.BillingAddress != string.Empty)
        {
            BillingAddress_Container.Visibility = Visibility.Visible;
            BillingAddress_Text.Text = creditcard.BillingAddress;
        }
        else
        {
            BillingAddress_Container.Visibility = Visibility.Collapsed;
        }

        // Country
        if (creditcard.Country != null && creditcard.Country != string.Empty)
        {
            Country_Container.Visibility = Visibility.Visible;
            Country_Text.Text = creditcard.Country;
        }
        else
        {
            Country_Container.Visibility = Visibility.Collapsed;
        }

        // State
        if (creditcard.State != null && creditcard.State != string.Empty)
        {
            State_Container.Visibility = Visibility.Visible;
            State_Text.Text = creditcard.State;
        }
        else
        {
            State_Container.Visibility = Visibility.Collapsed;
        }

        // City
        if (creditcard.City != null && creditcard.City != string.Empty)
        {
            City_Container.Visibility = Visibility.Visible;
            City_Text.Text = creditcard.City;
        }
        else
        {
            City_Container.Visibility = Visibility.Collapsed;
        }

        //Zip Code
        if (creditcard.ZipCode != null && creditcard.ZipCode != string.Empty)
        {
            ZipCode_Container.Visibility = Visibility.Visible;
            ZipCode_Text.Text = creditcard.ZipCode;
        }
        else
        {
            ZipCode_Container.Visibility = Visibility.Collapsed;
        }

        // Notes
        if (creditcard.Notes != null && creditcard.Notes != string.Empty)
        {
            Notes_Container.Visibility = Visibility.Visible;
            Notes_Text.Text = creditcard.Notes;
        }
        else
        {
            Notes_Container.Visibility = Visibility.Collapsed;
        }



    }

    //Updating The record
    private void Update_Button_Click(object sender, RoutedEventArgs e)
    {
        ErrorContainer_Grid.Visibility = Visibility.Collapsed;
        AddCreditCardContainer_Grid.Visibility = Visibility.Collapsed;
        ViewCreditCard_Grid.Visibility = Visibility.Visible;

        var creditcard = GetInputCard();

        if (creditcard == null)
        {
            return;
        }

        creditcard.Id = currentcreditcardId;

        CreditCardDL.UpdateCreditCard(creditcard);
        CreditCardDB.UpdateCreditCard(creditcard);

        RenderUserInterface(creditcard);
    }

    private void SearchBar_KeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (SearchBar.Text == string.Empty)
        {
            RefreshCreditCardsListView();
        }
        if (e.Key == Windows.System.VirtualKey.Enter)
        {

            var searchQuery = SearchBar.Text;

            if (searchQuery != string.Empty)
            {
                if (searchQuery.Length < 3)
                {
                    MessageDialogHelper.ShowMessageDialog(XamlRoot, "Error", "Search query must be at least 3 characters long.");
                    return;
                }

                var creditcards = CreditCardDL.GetCreditCards()
                    .Where(a =>
                        (!string.IsNullOrEmpty(a.CardHolderName) && a.CardHolderName.ToLower().Contains(searchQuery.ToLower())) ||
                        (!string.IsNullOrEmpty(a.CardNumber) && a.CardNumber.ToLower().Contains(searchQuery.ToLower())) ||
                        (!string.IsNullOrEmpty(a.CardType) && a.CardType.ToLower().Contains(searchQuery.ToLower())) ||
                        (!string.IsNullOrEmpty(a.CardIssuer) && a.CardIssuer.ToLower().Contains(searchQuery.ToLower()))
                )
                .ToList();

                if (creditcards.Count > 0)
                {
                    CreditCardsListView.Children.Clear();
                    radioButtons.Items.Clear();
                    currentcreditcardId = 0;
                    ViewCreditCard_Grid.Visibility = Visibility.Collapsed;
                    ErrorContainer_Grid.Visibility = Visibility.Visible;
                    AddCreditCardContainer_Grid.Visibility = Visibility.Collapsed;

                    foreach (var creditcard in creditcards)
                    {
                        AddCreditCardInListView(creditcard.Id, creditcard.CardIssuer, creditcard.CardNumber);
                    }
                }
                else
                {
                    MessageDialogHelper.ShowMessageDialog(XamlRoot, "No Results", "No accounts found matching the search query.");
                }
            }
        }
    }

    private void Modify_Button_Click(object sender, RoutedEventArgs e)
    {
        ErrorContainer_Grid.Visibility = Visibility.Collapsed;
        AddCreditCardContainer_Grid.Visibility = Visibility.Visible;
        ViewCreditCard_Grid.Visibility = Visibility.Collapsed;
        Save_Button.Visibility = Visibility.Collapsed;
        Update_Button.Visibility = Visibility.Visible;

        var creditcard = CreditCardDL.GetCreditCardById(currentcreditcardId);

        if (creditcard == null)
        {
            return;
        }

        CardHolderName_TextBox.Text = creditcard.CardHolderName;
        CardNumber_TextBox.Text = creditcard.CardNumber;
        ExpiryMonth_TextBox.Text = creditcard.ExpiryMonth;
        ExpiryYear_TextBox.Text = creditcard.ExpiryYear;
        CVV_TextBox.Text = creditcard.CVV;
        CardPin_TextBox.Text = creditcard.Pin;
        CardIssuer_TextBox.Text = creditcard.CardIssuer;
        CardType_TextBox.Text = creditcard.CardType;
        BillingAddress_TextBox.Text = creditcard.BillingAddress;
        Country_TextBox.Text = creditcard.Country;
        State_TextBox.Text = creditcard.State;
        City_TextBox.Text = creditcard.City;
        ZipCode_TextBox.Text = creditcard.ZipCode;
        Notes_TextBox.Text = creditcard.Notes;

    }

    //Deleting The Card
    private async void  Delete_Button_Click(object sender, RoutedEventArgs e)
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
            CreditCardDL.DeleteCreditCard(currentcreditcardId);
            CreditCardDB.DeleteCreditCard(currentcreditcardId);

            ViewCreditCard_Grid.Visibility = Visibility.Collapsed;
            ErrorContainer_Grid.Visibility = Visibility.Visible;

            RefreshCreditCardsListView();
}

    }

    // Clear All Fields (Add CreditCard Page)
    private void ClearFields()
    {
        CardHolderName_TextBox.Text = string.Empty;
        CardNumber_TextBox.Text = string.Empty;
        ExpiryMonth_TextBox.Text = string.Empty;
        ExpiryYear_TextBox.Text = string.Empty;
        CVV_TextBox.Text = string.Empty;
        CardPin_TextBox.Text = string.Empty;
        CardIssuer_TextBox.Text = string.Empty;
        CardType_TextBox.Text = string.Empty;
        BillingAddress_TextBox.Text = string.Empty;
        Country_TextBox.Text = string.Empty;
        State_TextBox.Text = string.Empty;
        City_TextBox.Text = string.Empty;
        ZipCode_TextBox.Text = string.Empty;
        Notes_TextBox.Text = string.Empty;
    }

    private CreditCard? GetInputCard()
    {
        var CardHolderName = CardHolderName_TextBox.Text;
        var CardNumber = CardNumber_TextBox.Text;
        var ExpiryMonth = ExpiryMonth_TextBox.Text;
        var ExpiryYear = ExpiryYear_TextBox.Text;
        var CardPin = CardPin_TextBox.Text;
        var CVV = CVV_TextBox.Text;
        var CardIssuer = CardIssuer_TextBox.Text;
        var CardType = CardType_TextBox.Text;
        var BillingAddress = BillingAddress_TextBox.Text;
        var Country = Country_TextBox.Text;
        var State = State_TextBox.Text;
        var City = City_TextBox.Text;
        var ZipCode = ZipCode_TextBox.Text;
        var Notes = Notes_TextBox.Text;

        var CreditCard = new CreditCard
            (
                Id: CreditCardDB.GetMaxId() + 1,
                CardHolderName: CardHolderName ?? "",
                CardNumber: CardNumber ?? "",
                ExpiryMonth: ExpiryMonth ?? "",
                ExpiryYear: ExpiryYear ?? "",
                Pin: CardPin ?? "",
                CVV: CVV ?? "",
                CardIssuer: CardIssuer ?? "",
                CardType: CardType ?? "",
                BillingAddress: BillingAddress ?? "",
                Country: Country ?? "",
                State: State ?? "",
                City: City ?? "",
                ZipCode: ZipCode ?? "",
                Notes: Notes ?? ""
            );
            return CreditCard;
    }


    private void CopyHolderName_Button_Click(object sender, RoutedEventArgs e)
    {
        var dataPackage = new DataPackage();
        dataPackage.SetText(CardHolderName_Text.Text);
        Clipboard.SetContent(dataPackage);
    }

    private void CopyCardNumber_Button_Click(object sender, RoutedEventArgs e)
    {
        var dataPackage = new DataPackage();
        dataPackage.SetText(CardNumber_Hidden.Text);
        Clipboard.SetContent(dataPackage);
    }

    private void CopyExpiryMonth_Button_Click(object sender, RoutedEventArgs e)
    {
        var dataPackage = new DataPackage();
        dataPackage.SetText(ExpiryMonth_Text.Text);
        Clipboard.SetContent(dataPackage);
    }

    private void CopyExpiryYear_Button_Click(object sender, RoutedEventArgs e)
    {
        var dataPackage = new DataPackage();
        dataPackage.SetText(ExpiryYear_Text.Text);
        Clipboard.SetContent(dataPackage);
    }

    private void CopyCVV_Button_Click(object sender, RoutedEventArgs e)
    {
        var dataPackage = new DataPackage();
        dataPackage.SetText(CVV_Hidden.Text);
        Clipboard.SetContent(dataPackage);
    }

    private void CopyCardPin_Button_Click(object sender, RoutedEventArgs e)
    {
        var dataPackage = new DataPackage();
        dataPackage.SetText(CardPin_Hidden.Text);
        Clipboard.SetContent(dataPackage);
    }

    private void CopyIssuer_Button_Click(object sender, RoutedEventArgs e)
    {
        var dataPackage = new DataPackage();
        dataPackage.SetText(CardIssuer_Text.Text);
        Clipboard.SetContent(dataPackage);
    }

    private void CopyCardType_Button_Click(object sender, RoutedEventArgs e)
    {
        var dataPackage = new DataPackage();
        dataPackage.SetText(CardType_Text.Text);
        Clipboard.SetContent(dataPackage);
    }

    private void CopyBillingAddress_Button_Click(object sender, RoutedEventArgs e)
    {
        var dataPackage = new DataPackage();
        dataPackage.SetText(BillingAddress_Text.Text);
        Clipboard.SetContent(dataPackage);
    }

    private void CopyCountry_Button_Click(object sender, RoutedEventArgs e)
    {
        var dataPackage = new DataPackage();
        dataPackage.SetText(Country_Text.Text);
        Clipboard.SetContent(dataPackage);
    }

    private void CopyState_Button_Click(object sender, RoutedEventArgs e)
    {
        var dataPackage = new DataPackage();
        dataPackage.SetText(State_Text.Text);
        Clipboard.SetContent(dataPackage);
    }

    private void CopyCity_Button_Click(object sender, RoutedEventArgs e)
    {
        var dataPackage = new DataPackage();
        dataPackage.SetText(City_Text.Text);
        Clipboard.SetContent(dataPackage);
    }

    private void CopyZipCode_Button_Click(object sender, RoutedEventArgs e)
    {
        var dataPackage = new DataPackage();
        dataPackage.SetText(ZipCode_Text.Text);
        Clipboard.SetContent(dataPackage);
    }

    private void CopyNotes_Button_Click(object sender, RoutedEventArgs e)
    {
        var dataPackage = new DataPackage();
        dataPackage.SetText(Notes_Text.Text);
        Clipboard.SetContent(dataPackage);
    }

    private void ToggleCardPin_Button_Click(object sender, RoutedEventArgs e)
    {
        if (ToggleCardPin_CheckBox.IsChecked == false)
        {
            CardPin_Text.Text = CardPin_Hidden.Text;
            ToggleCardPin_CheckBox.IsChecked = true;
            ToolTipService.SetToolTip(ToggleCardPin_Button, "Hide Card Pin");
            CardPinToggle_Icon.Glyph = "\uED1A";
        }
        else
        {
            CardPin_Text.Text = new string('●', CardPin_Hidden.Text.Length);
            ToggleCardPin_CheckBox.IsChecked = false;
            ToolTipService.SetToolTip(ToggleCardPin_Button, "Show Card Pin");
            CardPinToggle_Icon.Glyph = "\uE7B3";
        }
    }

    private void ToggleCVV_Button_Click(object sender, RoutedEventArgs e)
    {
        if (ToggleCVV_CheckBox.IsChecked == false)
        {
            CVV_Text.Text = CVV_Hidden.Text;
            ToggleCVV_CheckBox.IsChecked = true;
            ToolTipService.SetToolTip(ToggleCVV_Button, "Hide CVV");
            CVVToggle_Icon.Glyph = "\uED1A";
        }
        else
        {
            CVV_Text.Text = new string('●', CVV_Hidden.Text.Length);
            ToggleCVV_CheckBox.IsChecked = false;
            ToolTipService.SetToolTip(ToggleCVV_Button, "Show CVV");
            CVVToggle_Icon.Glyph = "\uE7B3";
        }
    }

    private void ToggleCardNumber_Button_Click(object sender, RoutedEventArgs e)
    {
        if (ToggleCardNumber_CheckBox.IsChecked == false)
        {
            CardNumber_Text.Text = CardNumber_Hidden.Text;
            ToggleCardNumber_CheckBox.IsChecked = true;
            ToolTipService.SetToolTip(ToggleCardNumber_Button, "Hide Card Number");
            CardNumberToggle_Icon.Glyph = "\uED1A";
        }
        else
        {
            CardNumber_Text.Text = new string('●', CardNumber_Hidden.Text.Length);
            ToggleCardNumber_CheckBox.IsChecked = false;
            ToolTipService.SetToolTip(ToggleCardNumber_Button, "Show Card Number");
            CardNumberToggle_Icon.Glyph = "\uE7B3";
        }
    }
}
