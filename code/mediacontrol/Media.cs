using System.ComponentModel;
using System;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Sandbox;

namespace Cinema;

public struct MediaRequest
{
    public string YouTubeId { get; set; }
    public IClient Requestor { get; set; }
}

public partial class Media : BaseNetworkable
{
    [Net]
    public string YouTubeId { get; set; }

    [Net]
    public IClient Requestor { get; set; }

    [Net]
    public string Title { get; set; }

    [Net]
    public int Duration { get; set; } = 0;

    public string DurationFormatted => TimeSpan.FromSeconds(Duration).ToString(@"hh\:mm\:ss");

    [Net]
    public string Thumbnail { get; set; }

    [Net]
    public bool CanEmbed { get; set; } = false;

    [Net]
    public bool Verified { get; set; } = false;

    public bool CanRemove(IClient client)
    {
        return client == Requestor;
    }

    public bool LocalClientCanRemove => Requestor == Game.LocalClient;

    public override string ToString()
    {
        return $"Media: {YouTubeId} ({Title}) by {Requestor.Name}";
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(YouTubeId, Requestor.Name, Title, Duration, Thumbnail, CanEmbed, Verified);
    }

    public static readonly string ApiUrl = "https://cinema-api.fly.dev";

    class ParseApiResponse
    {
        [JsonPropertyName("durationInSeconds")]
        public int DurationInSeconds { get; set; }
        [JsonPropertyName("title")]
        public string Title { get; set; }
        [JsonPropertyName("thumbnail")]
        public string Thumbnail { get; set; }
        [JsonPropertyName("canEmbed")]
        public bool CanEmbed { get; set; }
    }

    public static async Task<Media> CreateFromRequest(MediaRequest request)
    {
        ParseApiResponse response;

        try
        {
            response = await Http.RequestJsonAsync<ParseApiResponse>($"{ApiUrl}/api/parse2?type=yt&id={request.YouTubeId}", "GET");
        }
        catch (Exception e)
        {
            Log.Error(e.Message);
            return null;
        }

        var media = new Media()
        {
            Duration = response.DurationInSeconds,
            Title = response.Title,
            CanEmbed = response.CanEmbed,
            Thumbnail = response.Thumbnail,
            Verified = true,
            YouTubeId = request.YouTubeId,
            Requestor = request.Requestor
        };

        return media;
    }

    public static async Task<bool> VerifyYouTubeId(string youTubeId)
    {
        ParseApiResponse response;

        try
        {
            response = await Http.RequestJsonAsync<ParseApiResponse>($"{ApiUrl}/api/parse2?type=yt&id={youTubeId}", "GET");
        }
        catch (Exception e)
        {
            Log.Error(e.Message);
            return false;
        }

        return response.CanEmbed;
    }
}
