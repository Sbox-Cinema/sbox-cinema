using Sandbox.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using CinemaTeam.Plugins.Media.Yoinked;

using WebPanel = CinemaTeam.Plugins.Media.Yoinked.WebPanel;
using TextEntry = CinemaTeam.Plugins.Media.Yoinked.TextEntry;

namespace CinemaTeam.Plugins.Media;

public partial class BrowserMediaProviderPanel : MediaProviderHeaderPanel
{
    public Panel UrlTextEntry { get; set; }
    public Panel BrowserPanel { get; set; }
    public string DefaultUrl { get; set; } = "https://www.duckduckgo.com";
    public string DefaultScheme { get; set; } = "http://";

    protected List<string> BrowserHistory { get; set; } = new();
    protected int BrowserHistoryIndex { get; set; } = -1;

    private string LastKnownWebSurfaceUrl { get; set; }

    protected override void OnAfterTreeRender(bool firstTime)
    {
        if (!firstTime)
            return;

        var browser = BrowserPanel as WebPanel;
        browser.Surface.Url = DefaultUrl;
        OnUrlChanged(null, DefaultUrl);
        var textEntry = UrlTextEntry as TextEntry;
        textEntry.AddEventListener("onsubmit", OnClickNavigate);
    }

    protected override int BuildHash()
    {
        return HashCode.Combine(BrowserHistory.Count, BrowserHistoryIndex);
    }

    public override void Tick()
    {
        var browser = BrowserPanel as WebPanel;

        // Browser may be null for a bit after the panel is destroyed.
        if (!browser.IsValid)
            return;

        var currentUrl = browser.Surface.Url;
        // If the web surface URL changed on its own
        // (i.e. without clicking back or forward)
        if (currentUrl != LastKnownWebSurfaceUrl)
        {
            // When a WebSurface is created and first navigates to a URL,
            // it may have a null, garbage, or equivalent URL. 
            // We don't want to flood the browser history with these.
            var newUrlIsNull = browser.Surface.Url == null;
            var newUrlIsGarbage = browser.Surface.Url?.StartsWith("data:") ?? false;
            var newUrlIsEquivalent = browser.Surface.Url == LastKnownWebSurfaceUrl + "/";
            if (newUrlIsNull || newUrlIsGarbage || newUrlIsEquivalent)
                return;
            OnUrlChanged(LastKnownWebSurfaceUrl, currentUrl);
        }
    }
    
    private void Navigate(string url)
    {
        if (DefaultScheme != null && !url.Contains("://"))
        {
            url = DefaultScheme + url;
        }
        var browser = BrowserPanel as WebPanel;
        browser.Surface.Url = url;
        var urlText = UrlTextEntry as TextEntry;
        urlText.Text = url;
    }

    protected virtual void OnUrlChanged(string oldUrl, string newUrl)
    {
        if (newUrl == null)
            return;

        var urlText = UrlTextEntry as TextEntry;
        urlText.Text = newUrl;

        bool atEndOfHistory = BrowserHistoryIndex == BrowserHistory.Count - 1;
        // If we are overwriting the future...
        if (BrowserHistory.Any() && !atEndOfHistory)
        {
            // ...and the new url is not the next url in the history...
            if (BrowserHistory[BrowserHistoryIndex + 1] != newUrl)
            {
                // ...then destroy the future.
                BrowserHistory.RemoveRange(BrowserHistoryIndex + 1, BrowserHistory.Count - BrowserHistoryIndex - 1);
            }
        }
        BrowserHistoryIndex++;
        BrowserHistory.Add(newUrl);
        LastKnownWebSurfaceUrl = newUrl;
        Log.Trace($"Url changed, Browser URL: {(BrowserPanel as WebPanel).Surface.Url}, Old URL: {oldUrl}, New URL: {newUrl}, Browser history size: {BrowserHistory.Count}, Browser history index: {BrowserHistoryIndex}");
    }

    public virtual bool CanGoBack()
        => BrowserHistory.Any() && BrowserHistoryIndex > 0;

    public virtual bool CanGoForward()
        => BrowserHistoryIndex < BrowserHistory.Count - 1;
    
    public virtual void OnClickBack()
    {
        if (!CanGoBack())
            return;

        BrowserHistoryIndex--;
        var previousUrl = BrowserHistory[BrowserHistoryIndex];
        Navigate(previousUrl);
        LastKnownWebSurfaceUrl = previousUrl;
    }

    public virtual void OnClickForward()
    {
        if (!CanGoForward())
            return;

        BrowserHistoryIndex++;
        var nextUrl = BrowserHistory[BrowserHistoryIndex];
        Navigate(nextUrl); 
        LastKnownWebSurfaceUrl = nextUrl;
    }

    public virtual void OnClickNavigate()
    {
        var urlText = UrlTextEntry as TextEntry;
        Navigate(urlText.Text);
    }

    public virtual void OnClickHome()
    {
        var browser = BrowserPanel as WebPanel;
        browser.Surface.Url = DefaultUrl;
    }

    public virtual void OnClickAdd()
    {
        var urlText = UrlTextEntry as TextEntry;
        var url = urlText.Text;
        urlText.Text = "";
        urlText.Disabled = true;
        urlText.Disabled = false;
        OnRequestMedia(new IMediaSelector.MediaRequestEventArgs()
        {
            Query = url
        });
    }
}
