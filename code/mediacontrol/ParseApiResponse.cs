using System.Text.Json.Serialization;

namespace CinemaTeam.Plugins.Video;

public class ParseApiResponse
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
