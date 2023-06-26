namespace Cinema;

public partial class YouTube
{
    /// <summary>
    /// Defines key names used to refer to implementation specific data in
    /// <see cref="MediaRequest">MediaRequest</see>.
    /// </summary>
    public class RequestData
    {
        /// <summary>
        /// A key used to store the video ID of a YouTube video.
        /// </summary>
        public const string YouTubeId = nameof(YouTubeId);
        // TODO: Add CanEmbed?
        // TODO: Add Verified?
    }
}
