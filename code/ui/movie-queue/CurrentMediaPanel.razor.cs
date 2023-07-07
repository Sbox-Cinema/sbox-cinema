using CinemaTeam.Plugins.Video;
using Sandbox;
using Sandbox.UI;
using System;

namespace Cinema.UI;

public partial class CurrentMediaPanel : Panel
{
    public Panel Thumbnail { get; set; }

    public MediaController Controller { get; set; } = null;
    public MediaRequest NowPlaying => Controller?.CurrentMedia;
    public TimeSince TimeSinceStartedPlaying => Controller?.StartedPlaying ?? 0;
    public string NowPlayingTimeString => NowPlaying == null ? "0:00 / 0:00" : $"{TimeSpan.FromSeconds(TimeSinceStartedPlaying.Relative):hh\\:mm\\:ss} / {TimeSpan.FromSeconds(NowPlaying.GenericInfo.Duration):hh\\:mm\\:ss}";

    public CurrentMediaPanel()
    {
        if (Game.LocalPawn is not Player ply)
            return;

        var currentTheaterZone = ply.GetCurrentTheaterZone();

        if (currentTheaterZone == null)
            return;

        Controller = currentTheaterZone.MediaController;
    }

    protected override int BuildHash()
    {
        return HashCode.Combine(NowPlaying, TimeSinceStartedPlaying.Relative, Controller.IsPaused);
    }

    protected void OnTogglePause()
    {

        // TODO: Gray out this button if the player shouldn't be able to pause
        MediaController.TogglePauseMedia(Controller.Zone.NetworkIdent, Game.LocalClient.NetworkIdent);
    }
    protected void OnSkip()
    {
        MediaController.StopMedia(Controller.Zone.NetworkIdent, Game.LocalClient.NetworkIdent);
    }

    
}
