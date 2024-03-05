using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Storage.Pickers;

namespace Cyber_Vault;

public sealed partial class AccountThumbSelect : WindowEx
{
    public AccountThumbSelect()
    {
        InitializeComponent();

        IsTitleBarVisible = true;
        ExtendsContentIntoTitleBar = true;
        IsResizable = false;
        Height = 450;
        Width = 360;
        MaxHeight = 450;
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

    private void Fetch_Button_Click(object sender, RoutedEventArgs e)
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
            Debug.WriteLine("Selected file: " + file.Path);
            Browse_CheckBox.IsEnabled = true;
            Browse_CheckBox.IsChecked = true;
            Browse_CheckBox.Content = file.Name;

            Preview_Image.Source = new BitmapImage(new Uri(file.Path));
            Browse_FilePath.Text = file.Path;
            Browse_FileName.Text = file.Name;
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

    }
}
