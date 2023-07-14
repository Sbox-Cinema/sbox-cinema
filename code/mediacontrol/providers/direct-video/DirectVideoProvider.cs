using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaTeam.Plugins.Media;

public class DirectVideoProvider : IMediaProvider, IMediaSelector
{
    public string ProviderName => "Direct Video Player";

    public string ThumbnailPath => "ui/direct_video_icon.png";

    public MediaProviderHeaderPanel HeaderPanel => new BrowserMediaProviderPanel();

    public async Task<MediaRequest> CreateRequest(IClient client, string queryString)
    {
        var url = new Uri(queryString, UriKind.Absolute);
        var duration = await VideoPlayerUtilities.GetVideoDuration(url.ToString());
        var request = new MediaRequest()
        {
            GenericInfo = new()
            {
                Duration = (int)duration,
                Title = "Direct Video"
            }
        };
        request["Url"] = url.ToString();
        request.SetVideoProvider<DirectVideoProvider>();
        return request;
    }

    public async Task<IVideoPlayer> Play(MediaRequest requestData)
    {
        var player = new DirectVideoPlayer();
        await player.InitializePlayer(requestData);
        return player;
    }
}
