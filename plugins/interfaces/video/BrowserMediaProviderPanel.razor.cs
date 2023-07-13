using Sandbox.UI;

namespace CinemaTeam.Plugins.Video;

public partial class BrowserMediaProviderPanel : MediaProviderHeaderPanel
{
    public Panel UrlTextEntry { get; set; }
    public Panel BrowserPanel { get; set; }
    public string DefaultUrl { get; set; } = "https://www.google.com";

    private string PreviousUrl { get; set; }

    protected override void OnAfterTreeRender(bool firstTime)
    {
        if (!firstTime)
            return;

        var browser = BrowserPanel as WebPanel;
        browser.Surface.Url = "https://www.youtube.com";
    }

    public override void Tick()
    {
        base.Tick();

        var browser = BrowserPanel as WebPanel;
        if (browser.Surface.Url != PreviousUrl)
        {
            var urlText = UrlTextEntry as TextEntry;
            urlText.Text = browser?.Surface?.Url;
            PreviousUrl = browser?.Surface?.Url;
        }
    }

    public void OnClickAdd()
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
