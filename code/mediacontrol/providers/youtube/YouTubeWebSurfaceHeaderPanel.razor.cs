using Sandbox;
using Sandbox.UI;

namespace CinemaTeam.Plugins.Video;

public partial class YouTubeWebSurfaceHeaderPanel : MediaProviderHeaderPanel
{
    public Panel BrowserPanel { get; set; }

    protected override void OnAfterTreeRender(bool firstTime)
    {
        if (!firstTime)
            return;

        var browser = BrowserPanel as WebPanel;
        browser.Surface.Url = "https://www.youtube.com";
    }
}
