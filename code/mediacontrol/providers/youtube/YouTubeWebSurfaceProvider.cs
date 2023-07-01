using System;
using System.Threading.Tasks;

namespace CinemaTeam.Plugins.Video;

public class YouTubeWebSurfaceProvider : IVideoProvider
{
    public string ProviderName => "YouTube (WebSurface)";

    public MediaProviderHeaderPanel HeaderPanel => throw new NotImplementedException();

    public Task<IVideoPlayer> Play(MediaRequest requestData)
    {
        throw new NotImplementedException();
    }
}
