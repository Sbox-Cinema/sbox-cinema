using Sandbox.UI;
using System;

namespace CinemaTeam.Plugins.Media;
public abstract class MediaProviderHeaderPanel : Panel
{
    public event EventHandler<IMediaSelector.MediaRequestEventArgs> RequestMedia;

    protected void OnRequestMedia(IMediaSelector.MediaRequestEventArgs e)
    {
        RequestMedia.Invoke(this, e);
    }
}
