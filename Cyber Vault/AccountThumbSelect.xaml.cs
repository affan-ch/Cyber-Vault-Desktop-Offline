using System.Diagnostics;
using Cyber_Vault.DB;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Storage.Pickers;

namespace Cyber_Vault;

public sealed partial class AccountThumbSelect : WindowEx
{
    private string? _domain;
    public string? Domain
    {
        get => _domain;
        set
        {
            _domain = value;
            Domain_TextBox.Text = value;

            if (!string.IsNullOrEmpty(value))
            {
                if (AccountThumbsDB.IsThumbExist(value))
                {
                    MessageDisplay_TextBlock.Text = "A thumbnail for this domain already exists \nand will be replaced if you save a new one.";
                    Fetch_Button_Click(null, null);
                    Fetch_Button.Visibility = Visibility.Collapsed;
                    LoadPreviewImageAsync(value);
                }

            }
        }
    }

    private async void LoadPreviewImageAsync(string domain)
    {
        var bytes = AccountThumbsDB.GetImageBytes(domain);

        if (bytes != null)
        {
            using var stream = new MemoryStream(bytes);
            var bitmapImage = new BitmapImage();
            bitmapImage.DecodePixelWidth = 512;
            await bitmapImage.SetSourceAsync(stream.AsRandomAccessStream());
            Preview_Image.Source = bitmapImage;
        }
    }


    public AccountThumbSelect()
    {
        InitializeComponent();

        IsTitleBarVisible = true;
        ExtendsContentIntoTitleBar = true;
        IsResizable = false;
        Height = 470;
        Width = 360;
        MaxHeight = 470;
        MaxWidth = 360;
        IsMaximizable = false;
        IsMinimizable = false;
        IsShownInSwitchers = false;
        this.CenterOnScreen();
        Title = "Select Account Thumbnail";   
    }


    private void Preview_Container_PointerEntered(object sender, PointerRoutedEventArgs e)
    {
        Preview_Container.Background = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["LayerOnAcrylicFillColorDefaultBrush"];
    }

    private void Preview_Container_PointerExited(object sender, PointerRoutedEventArgs e)
    {
        Preview_Container.Background = (Microsoft.UI.Xaml.Media.Brush)Application.Current.Resources["DesktopAcrylicTransparentBrush"];
    }

    private void Fetch_Button_Click(object? sender, RoutedEventArgs? e)
    {
        if(string.IsNullOrEmpty(Domain_TextBox.Text))
        {        
            return;
        }

        var url1 = $"https://www.google.com/s2/favicons?domain={Domain_TextBox.Text}&sz=256";
        Image1.Source = new BitmapImage(new Uri(url1));

        var url2 = $"https://api.faviconkit.com/{Domain_TextBox.Text}/256";
        Image2.Source = new BitmapImage(new Uri(url2));

        var url3 = $"https://icon.horse/icon/{Domain_TextBox.Text}";
        Image3.Source = new BitmapImage(new Uri(url3));

        var url4 = $"https://logo.clearbit.com/{Domain_TextBox.Text}";
        Image4.Source = new BitmapImage(new Uri(url4));

        Images_Container.Visibility = Visibility.Visible;
        CheckBox_Container.Visibility = Visibility.Visible;

        CheckBox1.IsChecked = false;
        CheckBox2.IsChecked = false;
        CheckBox3.IsChecked = false;
        CheckBox4.IsChecked = false;
        Browse_CheckBox.IsChecked = false;
    }

    private async void Browse_Button_Click(object sender, RoutedEventArgs e)
    {
        var fileOpenPicker = new FileOpenPicker
        {
            ViewMode = PickerViewMode.Thumbnail,
            SuggestedStartLocation = PickerLocationId.PicturesLibrary
        };
        fileOpenPicker.FileTypeFilter.Add(".jpg");
        fileOpenPicker.FileTypeFilter.Add(".jpeg");
        fileOpenPicker.FileTypeFilter.Add(".png");

        // Get the current window's HWND by passing in the Window object
        var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);

        // Associate the HWND with the file picker
        WinRT.Interop.InitializeWithWindow.Initialize(fileOpenPicker, hwnd);

        var file = await fileOpenPicker.PickSingleFileAsync();

        if (file != null)
        {
            var fileName = file.Name;
            var extension = Path.GetExtension(fileName);
            Debug.WriteLine("Selected file: " + file.Path);

            if (extension != ".jpg" && extension != ".jpeg" && extension != ".png")
            {
                return;
            }
            
            Browse_CheckBox.IsEnabled = true;
            Browse_CheckBox.IsChecked = true;
            
            if (fileName.Length > 20)
            {            
                fileName = string.Concat(fileName.AsSpan()[..4], "...", fileName.AsSpan(fileName.Length - 8, 4), extension);
                Browse_CheckBox.Content = fileName;
            }
            else
            {
                Browse_CheckBox.Content = fileName;
            }

            Preview_Image.Source = new BitmapImage(new Uri(file.Path));
            Browse_FilePath.Text = file.Path;
            Browse_FileName.Text = file.Name;

            CheckBox1.IsChecked = false;
            CheckBox2.IsChecked = false;
            CheckBox3.IsChecked = false;
            CheckBox4.IsChecked = false;
        }
    }

    private void CheckBox1_Checked(object sender, RoutedEventArgs e)
    {
        CheckBox2.IsChecked = false;
        CheckBox3.IsChecked = false;
        CheckBox4.IsChecked = false;
        Browse_CheckBox.IsChecked = false;

        if(Image1.Source != null)
        {
            Preview_Image.Source = Image1.Source;
        }
    }

    private void CheckBox1_Unchecked(object sender, RoutedEventArgs e)
    {
        if(CheckBox2.IsChecked == false && CheckBox3.IsChecked == false && CheckBox4.IsChecked == false && Browse_CheckBox.IsChecked == false)
        {        
            Preview_Image.Source = new BitmapImage(new Uri("ms-appx:///Assets/cyber-vault-icon.png"));
        }
    }

    private void CheckBox2_Checked(object sender, RoutedEventArgs e)
    {
        CheckBox1.IsChecked = false;
        CheckBox3.IsChecked = false;
        CheckBox4.IsChecked = false;
        Browse_CheckBox.IsChecked = false;

        if (Image2.Source != null)
        {        
            Preview_Image.Source = Image2.Source;
        }
    }

    private void CheckBox2_Unchecked(object sender, RoutedEventArgs e)
    {
        if(CheckBox1.IsChecked == false && CheckBox3.IsChecked == false && CheckBox4.IsChecked == false && Browse_CheckBox.IsChecked == false)
        {               
            Preview_Image.Source = new BitmapImage(new Uri("ms-appx:///Assets/cyber-vault-icon.png"));
        }
    }

    private void CheckBox3_Checked(object sender, RoutedEventArgs e)
    {
        CheckBox1.IsChecked = false;
        CheckBox2.IsChecked = false;
        CheckBox4.IsChecked = false;
        Browse_CheckBox.IsChecked = false;

        if (Image3.Source != null)
        {        
            Preview_Image.Source = Image3.Source;
        }
    }

    private void CheckBox3_Unchecked(object sender, RoutedEventArgs e)
    {
        if(CheckBox1.IsChecked == false && CheckBox2.IsChecked == false && CheckBox4.IsChecked == false && Browse_CheckBox.IsChecked == false)
        {                      
            Preview_Image.Source = new BitmapImage(new Uri("ms-appx:///Assets/cyber-vault-icon.png"));
        }
    }

    private void CheckBox4_Checked(object sender, RoutedEventArgs e)
    {
        CheckBox1.IsChecked = false;
        CheckBox2.IsChecked = false;
        CheckBox3.IsChecked = false;
        Browse_CheckBox.IsChecked = false;

        if (Image4.Source != null)
        {               
            Preview_Image.Source = Image4.Source;
        }
    }

    private void CheckBox4_Unchecked(object sender, RoutedEventArgs e)
    {
        if(CheckBox1.IsChecked == false && CheckBox2.IsChecked == false && CheckBox3.IsChecked == false && Browse_CheckBox.IsChecked == false)
        {                             
            Preview_Image.Source = new BitmapImage(new Uri("ms-appx:///Assets/cyber-vault-icon.png"));
        }
    }

    private void Browse_CheckBox_Checked(object sender, RoutedEventArgs e)
    {
        CheckBox1.IsChecked = false;
        CheckBox2.IsChecked = false;
        CheckBox3.IsChecked = false;
        CheckBox4.IsChecked = false;

        if (!string.IsNullOrEmpty(Browse_FilePath.Text))
        {
            Preview_Image.Source = new BitmapImage(new Uri(Browse_FilePath.Text));
            Browse_CheckBox.Content = Browse_FileName.Text;
        }
    }

    private void Browse_CheckBox_Unchecked(object sender, RoutedEventArgs e)
    {
        if(CheckBox1.IsChecked == false && CheckBox2.IsChecked == false && CheckBox3.IsChecked == false && CheckBox4.IsChecked == false)
        {                                    
            Preview_Image.Source = new BitmapImage(new Uri("ms-appx:///Assets/cyber-vault-icon.png"));
        }
    }

    private void Save_Button_Click(object sender, RoutedEventArgs e)
    {
        if(Domain_TextBox.Text.Length == 0)
        {        
            MessageDisplay_TextBlock.Text = "* Please enter a domain name.";
            return;
        }

        if (CheckBox1.IsChecked == false && 
            CheckBox2.IsChecked == false && 
            CheckBox3.IsChecked == false && 
            CheckBox4.IsChecked == false && 
            Browse_CheckBox.IsChecked == false)
        {   
            MessageDisplay_TextBlock.Text = "* Please select a thumbnail or browse for one.";
            return;
        }

        var url1 = $"https://www.google.com/s2/favicons?domain={Domain_TextBox.Text}&sz=256";
        var url2 = $"https://api.faviconkit.com/{Domain_TextBox.Text}/256";
        var url3 = $"https://icon.horse/icon/{Domain_TextBox.Text}";
        var url4 = $"https://logo.clearbit.com/{Domain_TextBox.Text}";

        Save_Button.Content = "Saving...";
        Save_Button.IsEnabled = false;

        if(CheckBox1.IsChecked == true)
        {
            var isExist = AccountThumbsDB.IsThumbExist(Domain_TextBox.Text);

            if(isExist)
            {
                _ = AccountThumbsDB.UpdateThumbAsync(Domain_TextBox.Text, url1, AccountThumbsDB.ThumbType.Remote);
            }
            else
            {
                _ = AccountThumbsDB.StoreThumbAsync(Domain_TextBox.Text, url1, AccountThumbsDB.ThumbType.Remote);
            }
        }
        else if(CheckBox2.IsChecked == true)
        {        
            var isExist = AccountThumbsDB.IsThumbExist(Domain_TextBox.Text);
        
            if(isExist)
            {
                _ = AccountThumbsDB.UpdateThumbAsync(Domain_TextBox.Text, url2, AccountThumbsDB.ThumbType.Remote);
            }
            else
            {
                _ = AccountThumbsDB.StoreThumbAsync(Domain_TextBox.Text, url2, AccountThumbsDB.ThumbType.Remote);
            }
        }
        else if(CheckBox3.IsChecked == true)
        {        
            var isExist = AccountThumbsDB.IsThumbExist(Domain_TextBox.Text);
        
            if(isExist)
            {
                _ = AccountThumbsDB.UpdateThumbAsync(Domain_TextBox.Text, url3, AccountThumbsDB.ThumbType.Remote);
            }
            else
            {
                _ = AccountThumbsDB.StoreThumbAsync(Domain_TextBox.Text, url3, AccountThumbsDB.ThumbType.Remote);
            }
        }
        else if(CheckBox4.IsChecked == true)
        {
            var isExist = AccountThumbsDB.IsThumbExist(Domain_TextBox.Text);
        
            if(isExist)
            {
                _ = AccountThumbsDB.UpdateThumbAsync(Domain_TextBox.Text, url4, AccountThumbsDB.ThumbType.Remote);
            }
            else
            {
                _ = AccountThumbsDB.StoreThumbAsync(Domain_TextBox.Text, url4, AccountThumbsDB.ThumbType.Remote);
            }
        }
        else if(Browse_CheckBox.IsChecked == true)
        {
            var isExist = AccountThumbsDB.IsThumbExist(Domain_TextBox.Text);
        
            if(isExist)
            {
                _ = AccountThumbsDB.UpdateThumbAsync(Domain_TextBox.Text, Browse_FilePath.Text, AccountThumbsDB.ThumbType.Local);
            }
            else
            {
                _ = AccountThumbsDB.StoreThumbAsync(Domain_TextBox.Text, Browse_FilePath.Text, AccountThumbsDB.ThumbType.Local);
            }
        }

        Save_Button.IsEnabled = true;
        Save_Button.Content = "Save Thumbnail";
    }
}
