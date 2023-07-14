using MediaHelpers;
using Sandbox;

namespace CinemaTeam.Plugins.Media;

public class YouTubeVideoPlayer : DirectVideoPlayer 
{
    public override void OnFrame()
    {
        base.OnFrame();

        // When streaming a video from YouTube, it may hitch. This also occurs in
        // VLC, so it's not our fault. The workaround is to refresh the video.
        if (!IsPaused && VideoLoaded && VideoLastUpdated > 0.25f)
        {
            Log.Info("Video hitch detected. Seeking to current time.");
            VideoLoaded = false;
            Refresh();
        }
    }
}
