using System;

namespace CinemaTeam.Plugins.Video;

public partial class YouTubeDirect : IVideoProvider
{
    public string ProviderName => "YouTube (VideoPlayer)";

    public MediaProviderHeaderPanel HeaderPanel => throw new NotImplementedException();

    public event EventHandler<IVideoProvider.OnRequestMediaEventArgs> OnRequestMedia;

    public IVideoPlayer Play(string requestData)
    {
        return new YouTubeDirectPlayer(requestData);
    }
}
