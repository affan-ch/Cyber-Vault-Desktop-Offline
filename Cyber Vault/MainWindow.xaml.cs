using CommunityToolkit.Mvvm.Input;
using Cyber_Vault.DB;
using Cyber_Vault.Helpers;
using Cyber_Vault.Services;
using Cyber_Vault.Utils;
using Cyber_Vault.Views;
using H.NotifyIcon.EfficiencyMode;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using Windows.UI.ViewManagement;
using Cyber_Vault.DL;
namespace Cyber_Vault;

public sealed partial class MainWindow : WindowEx
{
    private readonly Microsoft.UI.Dispatching.DispatcherQueue dispatcherQueue;

    private readonly UISettings settings;

    public MainWindow()
    {
        InitializeComponent();

        AppWindow.SetIcon(Path.Combine(AppContext.BaseDirectory, "Assets/WindowIcon.ico"));

        Content = null;
        Title = "AppDisplayName".GetLocalized();
        //var icon = Icon.FromFile(Path.Combine(AppContext.BaseDirectory, "Assets/WindowIcon.ico"));
        //this.SetTaskBarIcon(icon);

        dispatcherQueue = Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread();
        settings = new UISettings();
        settings.ColorValuesChanged += Settings_ColorValuesChanged; // cannot use FrameworkElement.ActualThemeChanged event

        EfficiencyModeUtilities.SetEfficiencyMode(true);

        TrayIcon.ForceCreate(true); // Show System Tray Icon

        DatabaseHelper.CreateDatabase();

    }

    [RelayCommand]
    public async void ShowHideWindow()
    {
        var window = App.MainWindow;
        if (window == null)
        {
            return;
        }

        if (window.Visible)
        {
            // On Minimize to System Tray --> Delete MasterKey from Memory
            CredentialsManager.DeletePasswordFromMemory();

            // On Minimize to System Tray --> Logout
            UIElement? _login = App.GetService<HomePage>();
            App.MainWindow.Content = _login ?? new Frame();
            await ActivationService.StartupAsync();

            //  Clear the Account List
            AccountDL.ClearAccounts();

            // On Minimize to System Tray --> Hide Window
            window.Hide();
        }
        else
        {
            window.Show();
        }
    }

    [RelayCommand]
    public void ExitApplication()
    {
        AccountDL.ClearAccounts();
        CredentialsManager.DeleteUsernameFromMemory();
        CredentialsManager.DeletePasswordFromMemory();

        App.HandleClosedEvents = false;
        TrayIcon.Dispose();
        App.MainWindow?.Close();
    }

    // this handles updating the caption button colors correctly when indows system theme is changed
    // while the app is open
    private void Settings_ColorValuesChanged(UISettings sender, object args)
    {
        // This calls comes off-thread, hence we will need to dispatch it to current app's thread
        dispatcherQueue.TryEnqueue(() =>
        {
            TitleBarHelper.ApplySystemThemeToCaptionButtons();
        });
    }
}
