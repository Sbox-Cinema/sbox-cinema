using Sandbox.UI;
using System;

namespace CinemaTeam.Plugins.Media;
public abstract class MediaProviderHeaderPanel : Panel
{
    public event EventHandler<IMediaSelector.MediaRequestEventArgs> RequestMedia;
    public event EventHandler RequestClose;

    protected void OnRequestClose()
    {
        RequestClose?.Invoke(this, EventArgs.Empty);
    }

    protected void OnRequestMedia(IMediaSelector.MediaRequestEventArgs e)
    {
        RequestMedia.Invoke(this, e);
    }
}
