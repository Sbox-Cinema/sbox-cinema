using System;

namespace CinemaTeam.Plugins.Video;

public class YouTubeWebSurface : IVideoProvider
{
    public string ProviderName => "YouTube (WebSurface)";

    public MediaProviderHeaderPanel HeaderPanel => throw new NotImplementedException();

    public event EventHandler<IVideoProvider.OnRequestMediaEventArgs> OnRequestMedia;

    public IVideoPlayer Play(string requestData)
    {
        throw new NotImplementedException();
    }
}
