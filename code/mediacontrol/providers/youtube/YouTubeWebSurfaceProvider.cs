using System;
using System.Threading.Tasks;

namespace CinemaTeam.Plugins.Video;

public class YouTubeWebSurfaceProvider
{
    public string ProviderName => "YouTube (WebSurface)";
    public string ThumbnailPath => "assets/youtube_icon.png";

    public Task<IVideoPlayer> Play(MediaRequest requestData)
    {
        throw new NotImplementedException();
    }
}
