using CinemaTeam.Plugins.Video;
using Sandbox;
using Sandbox.UI;
using System;

namespace Cinema.UI;

public partial class CurrentMediaPanel : Panel
{
    public Panel Thumbnail { get; set; }
    public Panel VolumeSlider { get; set; }
    public Panel ProgressSlider { get; set; }

    public MediaController Controller { get; set; } = null;
    public MediaRequest NowPlaying => Controller?.CurrentMedia;
    public TimeSince TimeSinceStartedPlaying => Controller?.CurrentPlaybackTime ?? 0;
    public string NowPlayingTimeString => NowPlaying == null ? "0:00" : $"{TimeSpan.FromSeconds(TimeSinceStartedPlaying.Relative):hh\\:mm\\:ss}";
    public string ThumbnailPath => NowPlaying?.GenericInfo?.Thumbnail ?? "https://i.ytimg.com/vi/EbnH3VHzhu8/default.jpg";

    private bool _ProgressSliderClicked = false;

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
        var progress = ProgressSlider as SliderControl;
        progress.AddEventListener("onclick", _ => { Log.Info("left mouse down");  _ProgressSliderClicked = true; });
        progress.AddEventListener("onmouseup", 
            e =>
            {
                var mouseEvent = e as MousePanelEvent;
                if (mouseEvent.MouseButton == MouseButtons.Left)
                {
                    Log.Info("Left mouse up");
                    OnProgressChanged(progress.Value);
                };
            }
            );
        progress.AddEventListener("onmouseup",
            e =>
            {
                var mouseEvent = e as MousePanelEvent;
                if (mouseEvent.MouseButton == MouseButtons.Left)
                {
                    _ProgressSliderClicked = false;
                };
            }
            );


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
        MediaController.SeekMedia(Controller.Zone.NetworkIdent, Game.LocalClient.NetworkIdent, 0f);
    }

    protected void OnToggleMute()
    {
        MediaConfig.ShouldMute = !MediaConfig.ShouldMute;
    }

    protected void OnVolumeChanged(float volume)
    {
        MediaConfig.DefaultMediaVolume = volume;
    }

    public override void Tick()
    {
        if (!_ProgressSliderClicked)
        {
            var progress = ProgressSlider as SliderControl;
            progress.Value = Controller.CurrentPlaybackTime;
        }
    }

    protected void OnProgressChanged(float progress)
    {
        MediaController.SeekMedia(Controller.Zone.NetworkIdent, Game.LocalClient.NetworkIdent, progress);
    }
}
