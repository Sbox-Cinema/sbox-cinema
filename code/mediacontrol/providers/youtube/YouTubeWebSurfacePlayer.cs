namespace CinemaTeam.Plugins.Media;

public class YouTubeWebSurfacePlayer : WebSurfaceVideoPlayer
{
    public YouTubeWebSurfacePlayer(MediaRequest requestData) : base(requestData) { }

    public override bool IsPaused => false;
}
