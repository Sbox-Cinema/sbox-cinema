using Sandbox;
using System.Threading.Tasks;

namespace CinemaTeam.Plugins.Video;

public static class VideoPlayerUtilities
{
    /// <summary>
    /// Retrieve the duration in seconds of a video from a URL.
    /// </summary>
    /// <param name="url">The URL of a video such as a .mp4 or .webm</param>
    /// <returns>The video duration in seconds.</returns>
    public async static Task<float> GetVideoDuration(string url)
    {
        var isLoaded = false;
        var vid = new VideoPlayer();
        vid.OnLoaded += () => isLoaded = true;
        vid.Play(url);
        while (!isLoaded)
        {
            await GameTask.DelaySeconds(Time.Delta);
        }
        var duration = vid.Duration;
        vid.Dispose();
        return duration;
    }
}
