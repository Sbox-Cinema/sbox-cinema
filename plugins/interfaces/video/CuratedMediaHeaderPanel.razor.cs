namespace CinemaTeam.Plugins.Video;

public partial class CuratedMediaHeaderPanel : MediaProviderHeaderPanel
{
    public void OnPressButton()
    {
        OnRequestMedia(new IMediaSelector.MediaRequestEventArgs
        {
            Query = null
        });
    }
}
