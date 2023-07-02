using MediaHelpers;
using Sandbox.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaTeam.Plugins.Video;

public partial class YouTubeVideoProviderHeaderPanel : MediaProviderHeaderPanel
{
    public Panel UrlText { get; set; }
    public void OnAddUrl()
    {
        var textEntry = UrlText as TextEntry;
        var url = textEntry.Text;
        textEntry.Text = "";
        // Disable to force defocus. Don't enable again until after time has passed from awaiting.
        textEntry.Disabled = true;
        textEntry.Disabled = false;
        OnRequestMedia(new IMediaSelector.MediaRequestEventArgs() 
            { 
                Query = url
            });
    }
}
