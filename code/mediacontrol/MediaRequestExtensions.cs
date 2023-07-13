using Cinema;
using System.Threading.Tasks;

namespace CinemaTeam.Plugins.Video;

public static class MediaRequestExtensions
{
    public async static Task<IVideoPlayer> GetPlayer(this MediaRequest request)
    {
        // Log request URL and providre ID
        Log.Info($"Request URL: {request["Url"]}");
        Log.Info($"Request Provider ID: {request.VideoProviderId}");
        var provider = VideoProviderManager.Instance[request.VideoProviderId];
        return await provider.Play(request);
    }
}
