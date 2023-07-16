using System.Threading.Tasks;
using Sandbox;
using CinemaTeam.Plugins.Media;
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
    private IMediaPlayer CurrentMediaPlayer { get; set; }
    [Net, Change]
    public float CurrentPlaybackTime { get; private set; }
    [Net]
    public bool IsPaused { get; set; }

    public event EventHandler StartPlaying;
    public event EventHandler StopPlaying;

    [GameEvent.Tick.Server]
    private void OnTick()
    {
        if (CurrentMedia != null && CurrentPlaybackTime > CurrentMedia.GenericInfo?.Duration)
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
        if (shouldPause)
        {
            CurrentMediaPlayer.Controls?.Pause();
        }
        else
        {
            CurrentMediaPlayer.Controls?.Resume();
        }
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
            CurrentMediaPlayer.Controls?.Seek(CurrentPlaybackTime);
        }
    }

    private async void OnCurrentMediaChanged(MediaRequest oldValue, MediaRequest newValue)
    {
        Log.Trace($"Media changed to \"{newValue?.GenericInfo?.Title ?? "null"}\", requestor {newValue?.Requestor}, provider {newValue?.VideoProviderId}");
        if (newValue != null)
        {
            StartPlaying?.Invoke(this, null);
        }
        else
        {
            StopPlaying?.Invoke(this, null);
        }
        await ClientPlayMedia();
    }

    public void PlayMedia(MediaRequest media)
    {
        Log.Trace($"Old media. Network ident: {CurrentMedia?.NetworkIdent ?? 0}, provider: {CurrentMedia?.VideoProviderId ?? 0}, title: {CurrentMedia?.GenericInfo?.Title}");
        CurrentMedia = media;
        Log.Trace($"New media. Network ident: {media.NetworkIdent}, provider: {media.VideoProviderId}, title: {media.GenericInfo?.Title}");
        CurrentPlaybackTime = 0;
        IsPaused = false;
        StartPlaying?.Invoke(this, null);
    }

    [ClientRpc]
    private async Task ClientPlayMedia()
    {
        if (!Game.IsClient)
            return;

        CurrentMediaPlayer?.Stop();

        if (CurrentMedia == null)
        {
            Log.Trace("Current media: null");
            Projector?.SetMedia(null);
            return;
        }

        Log.Trace($"Current media: {CurrentMedia.GenericInfo?.Title}, provider ID: {CurrentMedia.VideoProviderId}");

        CurrentMediaPlayer = await CurrentMedia.GetPlayer();
        Projector?.SetMedia(CurrentMediaPlayer);
        if (CurrentMediaPlayer.AudioPlayer != null)
        {
            CurrentMediaPlayer.AudioPlayer.SetVolume(MediaConfig.DefaultMediaVolume);
            MediaConfig.DefaultMediaVolumeChanged += (_, volume) => CurrentMediaPlayer.AudioPlayer.SetVolume(volume);
            PlayAudio();
        }
    }

    private void PlayAudio()
    {
        if (CurrentMediaPlayer.AudioPlayer == null)
            return;

        var centerChannel = CinemaZone.AudioChannel.Center;
        // For now, we only support playing the center channel, but once VideoPlayer supports
        // multiple audio channels, we can play each channel at a different position in the world.
        if (Zone.HasSpeaker(centerChannel))
        {
            CurrentMediaPlayer.AudioPlayer.PlayAudio(Zone.GetSpeaker(centerChannel));
        }
        else
        {
            // Play a sound from the projector itself.
            CurrentMediaPlayer.AudioPlayer.PlayAudio(Projector);
        }
    }
}
