using Sandbox;
using System;
using System.Threading.Tasks;

namespace CinemaTeam.Plugins.Video;

public class YouTubeWebSurfaceProvider : IMediaProvider, IMediaSelector
{
    public string ProviderName => "YouTube (WebSurface)";
    public string ThumbnailPath => "ui/youtube_icon.png";

    public MediaProviderHeaderPanel HeaderPanel => new YouTubeWebSurfaceHeaderPanel();

    public Task<MediaRequest> CreateRequest(IClient client, string queryString)
    {
        throw new NotImplementedException();   
    }

    public Task<IVideoPlayer> Play(MediaRequest requestData)
    {
        IVideoPlayer player = new YouTubeWebSurfacePlayer(requestData);
        return Task.FromResult(player);
    }
}
