using MediaHelpers;
using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace CinemaTeam.Plugins.Media;

public partial class YouTubeVideoProvider : IMediaProvider, IMediaSelector
{
    public string ProviderName => "YouTube (VideoPlayer)";
    public string ThumbnailPath => "assets/youtube_icon_good.png";
    public MediaProviderHeaderPanel HeaderPanel 
        => new BrowserMediaProviderPanel() { DefaultUrl = "https://www.youtube.com" };

    public async Task<MediaRequest> CreateRequest(IClient client, string requestString)
    {
        var videoId = GetIdFromYoutubeUrl(requestString);
        var youtubePlayerResponse = await MediaHelper.GetYoutubePlayerResponseFromId(videoId);
        var mediaInfo = new MediaInfo()
        {
            Title = youtubePlayerResponse.Title,
            Author = youtubePlayerResponse.Author,
            Duration = (int)youtubePlayerResponse.DurationSeconds,
            Thumbnail = youtubePlayerResponse.Thumbnails.FirstOrDefault().Url
        };
        var request = new MediaRequest(mediaInfo);
        request.Requestor = client;
        request["Url"] = SelectBestStream(youtubePlayerResponse);
        request.SetVideoProvider<YouTubeVideoProvider>();
        return request;
    }

    public async Task<IMediaPlayer> Play(MediaRequest requestData)
    {
        var player = new VideoPlayerMediaAdapter();
        await player.StartAsync(requestData);
        return player;
    }
    private static string GetIdFromYoutubeUrl(string url)
    {
        url = url.Replace("shorts/", "watch?v=");
        return HttpUtility.ParseQueryString(new Uri(url).Query).Get("v");
    }


    private static string SelectBestStream(YoutubePlayerResponse response)
    {
        var qualityLabels = new[] { "1080p", "720p", "480p", "360p", "240p", "144p" };
        var streams = response.GetStreams();
        foreach (var stream in streams)
        {
            Log.Trace($"Stream: {stream.Bitrate}bps, {stream.VideoQualityLabel}, {stream.AudioCodec}, {stream.VideoWidth}x{stream.VideoHeight}");
        }
        IYoutubeStreamData bestStream = null;
        foreach (var quality in qualityLabels)
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
        Log.Trace($"Playing \"{bestStream?.VideoWidth}x{bestStream?.VideoHeight}, {bestStream?.VideoFramerate} FPS, bitrate {bestStream?.Bitrate}, audio codec {bestStream?.AudioCodec}: " + url);
        return url;
    }
}
