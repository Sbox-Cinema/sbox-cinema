using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaTeam.Plugins.Media;

public class DummyVideoProvider : IMediaProvider, IMediaSelector
{
    public string ProviderName => "Play Test Video";

    public string ThumbnailPath => "assets/dummy_video_player.png";

    public MediaProviderHeaderPanel HeaderPanel => new CuratedMediaHeaderPanel();

    public Task<MediaRequest> CreateRequest(IClient client, string queryString)
    {
        var request = new MediaRequest()
        {
            GenericInfo = new MediaInfo()
            {
                Author = "ducc",
                Title = $"{Guid.NewGuid()} Test Video For Test Purposes",
                Duration = 450,
                Thumbnail = null
            },
            Requestor = client
        };
        request.SetVideoProvider<DummyVideoProvider>();
        return Task.FromResult(request);
    }

    public Task<IMediaPlayer> Play(MediaRequest requestData)
    {
        IMediaPlayer player = new DummyVideoPresenter();
        return Task.FromResult(player);
    }
       
}
