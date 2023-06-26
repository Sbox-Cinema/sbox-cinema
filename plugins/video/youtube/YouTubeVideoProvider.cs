using System;

namespace CinemaTeam.Plugins.Video;

public partial class YouTubeVideoProvider : IVideoProvider
{
    public string ProviderName => "YouTube (VideoPlayer)";

    public MediaProviderHeaderPanel HeaderPanel => throw new NotImplementedException();

    public event EventHandler<IVideoProvider.OnRequestMediaEventArgs> OnRequestMedia;

    public IVideoControls Play(string requestData)
    {
        return new YouTubeVideoPlayer(requestData);
    }
}
