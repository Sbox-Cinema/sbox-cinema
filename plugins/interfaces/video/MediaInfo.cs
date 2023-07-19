using Sandbox;
using System;

namespace CinemaTeam.Plugins.Media;

/// <summary>
/// Contains information about media that is applicable to all providers.
/// </summary>
public partial class MediaInfo : BaseNetworkable, INetworkSerializer
{
    /// <summary>
    /// The title of the media.
    /// </summary>
    [Net]
    public string Title { get; set; }
    /// <summary>
    /// The author of the media; for example, an uploader.
    /// </summary>
    [Net]
    public string Author { get; set; }
    /// <summary>
    /// A URL that points to a thumbnail image used to represent the media.
    /// </summary>
    [Net]
    public string Thumbnail { get; set; }
    /// <summary>
    /// The duration of the media in seconds.
    /// </summary>
    [Net]
    public int Duration { get; set; } = 0;
    /// <summary>
    /// Gets the duration as a pretty, formatted string (hh:mm:ss).
    /// </summary>
    public string DurationFormatted => TimeSpan.FromSeconds(Duration).ToString(@"hh\:mm\:ss");

    public void Read(ref NetRead read)
    {
        Title = read.ReadString();
        Author = read.ReadString();
        Thumbnail = read.ReadString();
        Duration = read.Read<int>();
    }

    public void Write(NetWrite write)
    {
        write.Write(Title);
        write.Write(Author);
        write.Write(Thumbnail);
        write.Write(Duration);
    }
}
