using MediaHelpers;
using Sandbox;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaTeam.Plugins.Video;

public partial class YouTubeVideoProvider : IVideoProvider, IMediaCurator
{
    public string ProviderName => "YouTube (VideoPlayer)";

    public async Task<MediaRequest> GetRequest()
    {
        var request = new MediaRequest();
        const string nyanCat = "https://www.youtube.com/watch?v=QH2-TGUlwu4";
        const string ICBYDT = "https://www.youtube.com/watch?v=wKbU8B-QVZk";
        const string badApple = "https://www.youtube.com/watch?v=9lNZ_Rnr7Jc";
        var url = nyanCat;
        var videoId = MediaHelper.GetIdFromYoutubeUrl(url);
        var youtubePlayerResponse = await MediaHelper.GetYoutubePlayerResponse(videoId);
        var directVideoUrl = SelectBestStream(youtubePlayerResponse);
        request["Url"] = directVideoUrl;
        return request;
    }

    private string SelectBestStream(YoutubePlayerResponse response)
    {
        var qualityLabels = new[] {"1080p", "720p", "480p", "360p", "240p", "144p"};
        var streams = response.GetStreams();
        foreach (var stream in streams)
        {
            Log.Trace($"Stream: {stream.Bitrate}bps, {stream.VideoQualityLabel}, {stream.AudioCodec}, {stream.VideoWidth}x{stream.VideoHeight}");
        }
        IYoutubeStreamData bestStream = null;
        foreach(var quality in qualityLabels)
        {
            bestStream = streams
                .FirstOrDefault(s => s.VideoQualityLabel == quality && s.AudioCodec != null);
            if (bestStream == null)
            {
                continue;
            }
            else
            {
                break;
            }
        }
        if (bestStream == null)
        {
            bestStream = streams
                .OrderBy(s => s.VideoWidth)
                .FirstOrDefault(s => s.AudioCodec != null) ?? streams.FirstOrDefault();
        }
        var url = bestStream?.Url;
        Log.Trace($"Playing \"{bestStream?.VideoQualityLabel ?? null}\" quality video : " + url);
        return url;
    }

    public async Task<IVideoPlayer> Play(MediaRequest requestData)
    {
        var player = new YouTubeVideoPlayer(requestData);
        await player.InitializePlayer(requestData);
        return player;
    }
}
