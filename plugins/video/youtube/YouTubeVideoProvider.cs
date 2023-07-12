using MediaHelpers;
using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace CinemaTeam.Plugins.Video;

public partial class YouTubeVideoProvider : IMediaProvider, IMediaSelector, IMediaCurator
{
    public string ProviderName => "YouTube (VideoPlayer)";
    public string ThumbnailPath => "assets/youtube_icon_good.png";
    public MediaProviderHeaderPanel HeaderPanel => new YouTubeVideoProviderHeaderPanel();

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
        request.SetVideoProvider<YouTubeVideoProvider>();
        request.Requestor = client;
        request["Url"] = SelectBestStream(youtubePlayerResponse);
        return request;
    }

    private static List<string> GoodVideos = new()
    {
        "https://www.youtube.com/watch?v=QH2-TGUlwu4", // Nyan cat
        "https://www.youtube.com/watch?v=wKbU8B-QVZk", // I can't believe you've done this
        "https://www.youtube.com/watch?v=FtutLA63Cp8", // Bad Apple
        "https://www.youtube.com/watch?v=ygI-2F8ApUM", // BrodyQuest
        "https://www.youtube.com/watch?v=dQw4w9WgXcQ", // Never Gonna Give You Up
        "https://www.youtube.com/watch?v=irU_2h60T50", // Earthbound, Mr. Carpainter fight
        "https://www.youtube.com/watch?v=0ee0syZi9E0", // Royco Cup-a-Soup
        "https://www.youtube.com/watch?v=aaLrCdIsTPs", // Earthbound, Titanic Ant fight (in 4:3 resolution)
        "https://www.youtube.com/watch?v=eLbLKKlna00", // Eridium short (in vertical resolution)
        "https://www.youtube.com/watch?v=IIqtuupvdWg", // Test video (useful for testing borders)
    };

    public async Task<MediaRequest> SuggestMedia()
    {
        var randomVid = GoodVideos.OrderBy(_ => new Guid()).FirstOrDefault();
        return await CreateRequest(null, randomVid);
    }

    public async Task<IVideoPlayer> Play(MediaRequest requestData)
    {
        var player = new YouTubeVideoPlayer();
        await player.InitializePlayer(requestData);
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
