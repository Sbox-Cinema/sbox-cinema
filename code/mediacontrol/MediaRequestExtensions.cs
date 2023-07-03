using Cinema;
using System.Threading.Tasks;

namespace CinemaTeam.Plugins.Video;

public static class MediaRequestExtensions
{
    public async static Task<IVideoPlayer> GetPlayer(this MediaRequest request)
    {
        var provider = VideoProviderManager.Instance[request.VideoProviderId];
        return await provider.Play(request);
    }
}
