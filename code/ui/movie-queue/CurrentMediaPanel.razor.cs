using CinemaTeam.Plugins.Media;
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
    public MediaRating Rating { get; set; } = null;
    public MediaRequest NowPlaying => Controller?.CurrentMedia;
    public TimeSince TimeSinceStartedPlaying => Controller?.CurrentPlaybackPosition ?? 0;
    public string NowPlayingTimeString => NowPlaying == null ? "0:00" : $"{TimeSpan.FromSeconds(TimeSinceStartedPlaying.Relative):hh\\:mm\\:ss}";
    public string ThumbnailPath => NowPlaying?.GenericInfo?.Thumbnail ?? "https://i.ytimg.com/vi/EbnH3VHzhu8/default.jpg";
    /// <summary>
    /// Returns true if the current media is something that can be liked/disliked.
    /// </summary>
    public bool AllowSentimentVotes => NowPlaying != null;
    public string RatingButtonClass => MayRateVideo ? "" : "disabled";
    private bool MayRateVideo => AllowSentimentVotes && NowPlaying?.Requestor != Game.LocalClient;

    private bool _ProgressSliderClicked = false;

    public CurrentMediaPanel()
    {
        if (Game.LocalPawn is not Player ply)
            return;

        var currentTheaterZone = ply.GetCurrentTheaterZone();

        if (currentTheaterZone == null)
            return;

        Controller = currentTheaterZone.MediaController;
        Rating = currentTheaterZone.MediaRating;
    }

    protected override void OnAfterTreeRender(bool firstTime)
    {
        if (!firstTime)
            return;

        var volumeSlider = VolumeSlider as SliderControl;
        MediaConfig.DefaultMediaVolumeChanged += (_, volume) => volumeSlider.Value = volume;
        volumeSlider.Value = MediaConfig.DefaultMediaVolume;
        var progress = ProgressSlider as SliderControl;
        progress.AddEventListener("onclick", _ => { _ProgressSliderClicked = true; });
        progress.AddEventListener("onmouseup", 
            e =>
            {
                var mouseEvent = e as MousePanelEvent;
                if (mouseEvent.MouseButton == MouseButtons.Left)
                {
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
        var combinedRatinghash = 0;
        // If any vote is changed, the hash should change.
        foreach(var rating in Rating.GetAllRatings())
        {
            var ratingHash = HashCode.Combine(rating.Key, rating.Value);
            combinedRatinghash = HashCode.Combine(combinedRatinghash, ratingHash);
        }
        return HashCode.Combine(combinedRatinghash, NowPlaying, TimeSinceStartedPlaying.Relative, Controller.IsPaused);
    }

    protected void OnTogglePause()
    {

        // TODO: Gray out this button if the player shouldn't be able to pause
        MediaController.TogglePauseMedia(Controller.Zone.NetworkIdent, Game.LocalClient.NetworkIdent);
    }

    protected string GetLikeClass() => HasLiked ? "selected" : "";
    protected string GetDislikeClass() => HasDisliked ? "selected" : "";
    protected bool HasLiked => Rating.HasRated(Game.LocalClient, true);
    protected bool HasDisliked => Rating.HasRated(Game.LocalClient, false);

    protected void OnLike()
    {
        if (HasLiked)
        {
            Rating.RemoveRating(Game.LocalClient);
        }
        else
        {
            Rating.AddRating(Game.LocalClient, true);
        }
    }
    protected void OnDislike()
    {
        if (HasDisliked)
        {
            Rating.RemoveRating(Game.LocalClient);
        }
        else
        {
            Rating.AddRating(Game.LocalClient, false);
        }
    }

    protected void OnSkip()
    {
        MediaController.StopMedia(Controller.Zone.NetworkIdent, Game.LocalClient.NetworkIdent);
    }

    protected void OnRestart()
    {
        Controller.SeekMedia(Game.LocalClient, 0f);
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
            progress.Value = Controller.CurrentPlaybackPosition;
        }
    }

    protected void OnProgressChanged(float progress)
    {
        Controller.SeekMedia(Game.LocalClient, progress);
    }
}
