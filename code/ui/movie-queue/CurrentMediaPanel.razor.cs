using CinemaTeam.Plugins.Video;
using Sandbox;
using Sandbox.UI;
using System;

namespace Cinema.UI;

public partial class CurrentMediaPanel : Panel
{
    public Panel Thumbnail { get; set; }
    public Panel VolumeSlider { get; set; }

    public MediaController Controller { get; set; } = null;
    public MediaRequest NowPlaying => Controller?.CurrentMedia;
    public TimeSince TimeSinceStartedPlaying => Controller?.TimeSinceStartedPlaying ?? 0;
    public string NowPlayingTimeString => NowPlaying == null ? "0:00" : $"{TimeSpan.FromSeconds(TimeSinceStartedPlaying.Relative):hh\\:mm\\:ss}";

    public CurrentMediaPanel()
    {
        if (Game.LocalPawn is not Player ply)
            return;

        var currentTheaterZone = ply.GetCurrentTheaterZone();

        if (currentTheaterZone == null)
            return;

        Controller = currentTheaterZone.MediaController;

    }

    protected override void OnAfterTreeRender(bool firstTime)
    {
        if (!firstTime)
            return;

        var volumeSlider = VolumeSlider as SliderControl;
        MediaConfig.DefaultMediaVolumeChanged += (_, volume) => volumeSlider.Value = volume;
        volumeSlider.Value = MediaConfig.DefaultMediaVolume;
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

    protected void OnRestart()
    {

    }

    protected void OnVolumeChanged(float volume)
    {
        MediaConfig.DefaultMediaVolume = volume;
    }

    
}
