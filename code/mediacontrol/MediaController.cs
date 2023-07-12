using System.Threading.Tasks;
using Sandbox;
using CinemaTeam.Plugins.Video;
using System;

namespace Cinema;

/// <summary>
/// Manages the presentation and playback state of the currently selected media 
/// in a CinemaZone. It is called by the media queue, and calls the projector and
/// speakers of a CinemaZone.
/// </summary>
public partial class MediaController : EntityComponent<CinemaZone>, ISingletonComponent
{
    public CinemaZone Zone => Entity;
    public ProjectorEntity Projector => Zone.ProjectorEntity;

    [Net, Change]
    public MediaRequest CurrentMedia { get; set; }
    private IVideoPlayer CurrentVideoPlayer { get; set; }
    [Net, Change]
    public float CurrentPlaybackTime { get; private set; }
    [Net]
    public bool IsPaused { get; set; }

    public event EventHandler StartPlaying;
    public event EventHandler StopPlaying;

    [GameEvent.Tick.Server]
    private void OnTick()
    {
        if (CurrentMedia != null && CurrentPlaybackTime > CurrentMedia.GenericInfo.Duration)
        {
            StopMedia(null);
        }
        if (CurrentMedia != null && !IsPaused)
        {
            CurrentPlaybackTime += Time.Delta;
        }
    }

    public static MediaController FindByZoneId(int zoneId)
    {
        var zone = Sandbox.Entity.FindByIndex(zoneId) as CinemaZone;
        var controller = zone.MediaController;
        return controller;
    }

    [ConCmd.Server]
    public static void TogglePauseMedia(int zoneId, int clientId)
    {
        var controller = FindByZoneId(zoneId);
        var client = ClientHelper.FindById(clientId);
        controller.TogglePauseMedia(client);
    }

    public void TogglePauseMedia(IClient client)
    {

        // TODO: Return early if the client is not allowed to pause the media.
        IsPaused = !IsPaused;
        SetPauseMedia(IsPaused);
    }

    [ConCmd.Server]
    public static void SeekMedia(int zoneId, int clientId, float time)
    {
        var controller = FindByZoneId(zoneId);
        // TODO: Verify whether client is allowed to seek.
        // Due to ChangeAttribute, all clients should now seek to the new position.
        controller.CurrentPlaybackTime = time;
    }

    [ClientRpc]
    public void SetPauseMedia(bool shouldPause)
    {
        CurrentVideoPlayer.SetPaused(shouldPause);
    }

    [ConCmd.Server]
    public static void StopMedia(int zoneId, int clientId)
    {
        var controller = FindByZoneId(zoneId);
        var client = ClientHelper.FindById(clientId);
        controller.StopMedia(client);
    }

    public void StopMedia(IClient client)
    {
        CurrentMedia = null;
        StopPlaying?.Invoke(this, null);
    }

    private void OnCurrentPlaybackTimeChanged(float oldValue, float newValue)
    {
        // If the server's playback position has been changed by at least one second,
        // all of the clients should seek to the new position.
        if (Math.Abs(newValue - oldValue) >= 1f)
        {
            CurrentVideoPlayer.Seek(CurrentPlaybackTime);
        }
    }

    private async void OnCurrentMediaChanged(MediaRequest oldValue, MediaRequest newValue)
    {
        if (newValue != null)
        {
            StartPlaying?.Invoke(this, null);
        }
        else
        {
            StopPlaying?.Invoke(this, null);
        }
        await PlayCurrentMedia();
    }

    public void PlayMedia(MediaRequest media)
    {
        CurrentMedia = media;
        CurrentPlaybackTime = 0;
        IsPaused = false;
        StartPlaying?.Invoke(this, null);
    }

    [ClientRpc]
    private async Task PlayCurrentMedia()
    {
        if (!Game.IsClient)
            return;

        CurrentVideoPlayer?.Stop();
        StopAudio();

        if (CurrentMedia == null)
        {
            Projector?.SetMedia(null);
            return;
        }

        CurrentVideoPlayer = await CurrentMedia.GetPlayer();
        CurrentVideoPlayer.SetVolume(MediaConfig.DefaultMediaVolume);
        MediaConfig.DefaultMediaVolumeChanged += (_, volume) => CurrentVideoPlayer.SetVolume(volume);
        Projector?.SetMedia(CurrentVideoPlayer);
        PlayAudio();
    }

    private void PlayAudio()
    {
        var centerChannel = CinemaZone.AudioChannel.Center;
        // For now, we only support playing the center channel, but once VideoPlayer supports
        // multiple audio channels, we can play each channel at a different position in the world.
        if (Zone.HasSpeaker(centerChannel))
        {
            CurrentVideoPlayer.PlayAudio(Zone.GetSpeaker(centerChannel));
        }
        else
        {
            // Play a sound somewhere over the audience's heads.
            Projector?.PlayOverheadAudio();
        }
    }

    private void StopAudio()
    {
        Zone.StopAllSpeakerAudio();
        Projector?.StopOverheadAudio();
    }

}
