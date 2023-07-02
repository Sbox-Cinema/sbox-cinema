using MediaHelpers;
using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaTeam.Plugins.Video;

public partial class YouTubeVideoProvider : IMediaProvider, IMediaSelector, IMediaCurator
{
    public string ProviderName => "YouTube (VideoPlayer)";
    public string ThumbnailPath => "assets/youtube_icon.png";
    public MediaProviderHeaderPanel HeaderPanel => new YouTubeVideoProviderHeaderPanel();

    public async Task<MediaRequest> CreateRequest(IClient client, string requestString)
    {
        var videoId = MediaHelper.GetIdFromYoutubeUrl(requestString);
        var youtubePlayerResponse = await MediaHelper.GetYoutubePlayerResponse(videoId);
        var mediaInfo = new MediaInfo()
        {
            Title = youtubePlayerResponse.Title,
            Author = youtubePlayerResponse.Author,
            Duration = (int)youtubePlayerResponse.Duration.Value.TotalSeconds,
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
    };

    // These next two videos have an issue where the texture for the video will be 1x1 pixels.
    // Audio plays fine, though. Not sure where the fault lies.
    private const string IssuePlokBeach = "https://www.youtube.com/watch?v=9gpu1GHRymY";
    private const string IssueRoyco = "https://www.youtube.com/watch?v=0ee0syZi9E0"; 

    public async Task<MediaRequest> SuggestMedia()
    {
        var randomVid = GoodVideos.OrderBy(_ => new Guid()).FirstOrDefault();
        return await CreateRequest(null, randomVid);
    }

    public async Task<IVideoPlayer> Play(MediaRequest requestData)
    {
        var player = new YouTubeVideoPlayer(requestData);
        await player.InitializePlayer(requestData);
        return player;
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
        Log.Trace($"Playing \"{bestStream?.VideoQualityLabel ?? null}\" quality video : " + url);
        return url;
    }
}
