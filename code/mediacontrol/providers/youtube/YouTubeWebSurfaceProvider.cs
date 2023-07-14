using Sandbox;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace CinemaTeam.Plugins.Media;

public class YouTubeWebSurfaceProvider : IMediaProvider, IMediaSelector
{
    public string ProviderName => "YouTube (WebSurface)";
    public string ThumbnailPath => "ui/youtube_icon.png";

    public MediaProviderHeaderPanel HeaderPanel 
        => new BrowserMediaProviderPanel() { DefaultUrl = "https://www.youtube.com" };

    public async Task<MediaRequest> CreateRequest(IClient client, string queryString)
    {
        // TODO: Use Media Helpers to get the video information.
        var request = new MediaRequest()
        {
            Requestor = client,
        };
        request["Url"] = queryString;
        request.SetVideoProvider<YouTubeWebSurfaceProvider>();
        return await GameTask.FromResult(request);
    }

    public async Task<IMediaPlayer> Play(MediaRequest requestData)
    {
        var player = new YouTubeWebSurfacePlayer();
        await player.StartAsync(requestData);
        return player;
    }
}
