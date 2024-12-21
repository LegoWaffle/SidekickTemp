using System.ComponentModel;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Media;
using Microsoft.Extensions.Logging;
using Microsoft.Web.WebView2.Core;
using Sidekick.Apis.Poe.Clients;
using Sidekick.Apis.Poe.CloudFlare;
using Application=System.Windows.Application;

namespace Sidekick.Wpf;

public partial class CloudflareWindow
{
    private readonly ILogger logger;
    private readonly ICloudflareService cloudflareService;
    private readonly Uri uri;
    private bool challengeCompleted;

    public CloudflareWindow(ILogger logger, ICloudflareService cloudflareService, Uri uri)
    {
        this.logger = logger;
        this.cloudflareService = cloudflareService;
        this.uri = uri;
        InitializeComponent();
        Ready();
    }

    public void Ready()
    {
        _ = Application.Current.Dispatcher.Invoke(async () =>
        {
            Topmost = true;
            ShowInTaskbar = true;
            ResizeMode = ResizeMode.NoResize;

            await WebView.EnsureCoreWebView2Async();
            WebView.CoreWebView2.Settings.UserAgent = PoeTradeHandler.UserAgent;

            // Handle cookie changes by checking cookies after navigation
            WebView.CoreWebView2.NavigationCompleted += CoreWebView2_NavigationCompleted;

            WebView.Source = uri;

            // This avoids the white flicker which is caused by the page content not being loaded initially. We show the webview control only when the content is ready.
            WebView.Visibility = Visibility.Visible;

            // The window background is transparent to avoid any flickering when opening a window. When the webview content is ready we need to set a background color. Otherwise, mouse clicks will go through the window.
            Background = (Brush?)new BrushConverter().ConvertFrom("#000000");
            Opacity = 0.01;

            CenterOnScreen();
            Activate();
        });
    }

    private void CenterOnScreen()
    {
        // Get the window's handle
        var windowHandle = new WindowInteropHelper(this).Handle;

        // Get the screen containing the window
        var currentScreen = Screen.FromHandle(windowHandle);

        // Get the working area of the screen (excluding taskbar, DPI-aware)
        var workingArea = currentScreen.WorkingArea;

        // Get the DPI scaling factor for the monitor
        var dpi = VisualTreeHelper.GetDpi(this);

        // Convert physical pixels (from working area) to WPF device-independent units (DIPs)
        var workingAreaWidthInDips = workingArea.Width / (dpi.PixelsPerInchX / 96.0);
        var workingAreaHeightInDips = workingArea.Height / (dpi.PixelsPerInchY / 96.0);
        var workingAreaLeftInDips = workingArea.Left / (dpi.PixelsPerInchX / 96.0);
        var workingAreaTopInDips = workingArea.Top / (dpi.PixelsPerInchY / 96.0);

        // Get the actual size of the window in DIPs
        var actualWidth = Width;
        var actualHeight = Height;

        // Calculate centered position within the working area
        var left = workingAreaLeftInDips + (workingAreaWidthInDips - actualWidth) / 2;
        var top = workingAreaTopInDips + (workingAreaHeightInDips - actualHeight) / 2;

        // Set the window's position
        Left = left;
        Top = top;
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        if (!challengeCompleted)
        {
            logger.LogInformation("[CloudflareWindow] Closing the window without completing the challenge, marking as failed");
            _ = cloudflareService.CaptchaChallengeFailed();
        }

        base.OnClosing(e);
    }

    private async void CoreWebView2_NavigationCompleted(object? sender, CoreWebView2NavigationCompletedEventArgs e)
    {
        try
        {
            var cookies = await WebView.CoreWebView2.CookieManager.GetCookiesAsync(uri.GetLeftPart(UriPartial.Authority));
            var cfCookie = cookies.FirstOrDefault(c => c.Name == "cf_clearance");
            if (cfCookie == null)
            {
                return;
            }

            // Store the Cloudflare cookie
            challengeCompleted = true;
            _ = cloudflareService.CaptchaChallengeCompleted(cookies.ToDictionary(c => c.Name, c => c.Value));
            logger.LogInformation("[CloudflareWindow] Cookie check completed, challenge likely completed");

            Dispatcher.Invoke(Close);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "[CloudflareWindow] Error handling cookie check");
        }
    }
}
