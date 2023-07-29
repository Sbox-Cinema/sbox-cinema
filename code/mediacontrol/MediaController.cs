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
    public class MediaStartedEventArgs : EventArgs
    {
        public MediaStartedEventArgs(MediaRequest request)
        {
            Request = request;
        }

        public MediaRequest Request { get; init; }
    }

    public class MediaStoppedEventArgs : EventArgs
    {
        public MediaStoppedEventArgs(MediaRequest request, float watchTime)
        {
            Request = request;
            WatchTime = watchTime;
        }

        public MediaRequest Request { get; init; }
        public float WatchTime { get; init; }
    }

    public CinemaZone Zone => Entity;
    public ProjectorEntity Projector => Zone.ProjectorEntity;

    [Net, Change]
    public MediaRequest CurrentMedia { get; set; }
    private IMediaPlayer CurrentMediaPlayer { get; set; }
    [Net, Change]
    public float CurrentPlaybackPosition { get; private set; }
    /// <summary>
    /// The total playback time of the current media. Unlike <c>CurrentPlaybackPosition</c>,
    /// this value does not get reset when seeking. This is useful for granting rewards based
    /// on actual watch time.
    /// </summary>
    [Net]
    public float CurrentWatchTime { get; private set; }
    [Net]
    public bool IsPaused { get; set; }

    public event EventHandler<MediaStartedEventArgs> StartPlaying;
    public event EventHandler<MediaStoppedEventArgs> StopPlaying;

    [GameEvent.Tick.Server]
    protected void OnServerTick()
    {
        if (CurrentMedia != null && CurrentPlaybackPosition > CurrentMedia.GenericInfo?.Duration)
        {
            StopMedia(null);
        }
        if (CurrentMedia != null && !IsPaused)
        {
            CurrentPlaybackPosition += Time.Delta;
            CurrentWatchTime += Time.Delta;
        }
    }

    [GameEvent.Tick.Client]
    protected void OnClientTick()
    {
        if (Game.LocalPawn is not Player ply)
            return;

        // Don't check for desyncs if the player is not in the theater zone.
        if (ply.GetCurrentTheaterZone() != Zone)
            return;

        FixDesync();
    }

    private void FixDesync()
    {
        // If the current media is more than a second out of sync with the server, seek.
        if (CurrentMediaPlayer?.Controls != null
            && Math.Abs(CurrentMediaPlayer.Controls.PlaybackTime - CurrentPlaybackPosition) >= 1f)
        {
            Log.Trace($"Desync detected, seeking. Server Time: {CurrentPlaybackPosition} Client time: {CurrentMediaPlayer.Controls.PlaybackTime}");
            CurrentMediaPlayer.Controls.Seek(CurrentPlaybackPosition);
        }
    }

    [ConCmd.Client("media.playback.info")]
    protected static void DumpPlaybackInfoCmd()
    {
        if (Game.LocalPawn is not Player ply)
            return;

        ply.GetCurrentTheaterZone()?.MediaController?.DumpPlaybackInfo();
    }

    protected void DumpPlaybackInfo()
    {
        Log.Info($"Server Time: {CurrentPlaybackPosition} Client time: {CurrentMediaPlayer.Controls.PlaybackTime}");
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
        var client = ClientHelper.FindById(clientId);
        controller.SeekMedia(client, time);
    }

    public void SeekMedia(IClient client, float time)
    {
        if (Game.IsClient)
        {
            SeekMedia(Entity.NetworkIdent, client.NetworkIdent, time);
            return;
        }
        Log.Info($"{Entity.Name}: Seek to {time} requested by {client.ToString() ?? "null"}");
        // TODO: Verify whether client is allowed to seek.
        // Due to ChangeAttribute, all clients should now seek to the new position.
        CurrentPlaybackPosition = time;
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
        StopPlaying?.Invoke(this, new MediaStoppedEventArgs(CurrentMedia, CurrentWatchTime));
        CurrentMedia = null;
        CurrentWatchTime = 0;
    }

    private void OnCurrentPlaybackPositionChanged(float oldValue, float newValue)
    {
        // If the server's playback position has been changed by at least one second,
        // all of the clients should seek to the new position.
        if (Math.Abs(newValue - oldValue) >= 1f)
        {
            CurrentMediaPlayer.Controls?.Seek(CurrentPlaybackPosition);
        }
    }

    private async void OnCurrentMediaChanged(MediaRequest oldValue, MediaRequest newValue)
    {
        Log.Trace($"Media changed to \"{newValue?.GenericInfo?.Title ?? "null"}\", requestor {newValue?.Requestor}, provider {newValue?.VideoProviderId}");
        if (newValue != null)
        {
            StartPlaying?.Invoke(this, new MediaStartedEventArgs(newValue));
        }
        else
        {
            StopPlaying?.Invoke(this, new MediaStoppedEventArgs(oldValue, CurrentWatchTime));
        }
        await ClientPlayMedia();
    }

    public void PlayMedia(MediaRequest media)
    {
        Log.Trace($"Old media. Network ident: {CurrentMedia?.NetworkIdent ?? 0}, provider: {CurrentMedia?.VideoProviderId ?? 0}, title: {CurrentMedia?.GenericInfo?.Title}");
        CurrentMedia = media;
        Log.Trace($"New media. Network ident: {media.NetworkIdent}, provider: {media.VideoProviderId}, title: {media.GenericInfo?.Title}");
        CurrentPlaybackPosition = 0;
        CurrentWatchTime = 0;
        IsPaused = false;
        StartPlaying?.Invoke(this, new MediaStartedEventArgs(CurrentMedia));
    }

    /// <summary>
    /// Get a media player for <c>CurrentMedia</c> and set it on the projector. 
    /// This method gets called when <c>CurrentMedia</c> changes on the server, or
    /// when entering a theater zone where media is already playing.
    /// </summary>
    [ClientRpc]
    internal async Task ClientPlayMedia()
    {
        if (Game.LocalPawn is not Player ply)
            return;

        // Don't play the new media if the player is not in the theater zone.
        // This will get updated when the player enters the zone.
        if (ply.GetCurrentTheaterZone() != Zone)
            return;

        CurrentMediaPlayer?.Stop();

        if (CurrentMedia == null)
        {
            Log.Trace($"{Entity.Name} - Current media: null");
            Projector?.SetMedia(null);
            return;
        }

        Log.Trace($"{Entity.Name} - Current media: {CurrentMedia.GenericInfo?.Title}, provider ID: {CurrentMedia.VideoProviderId}");

        CurrentMediaPlayer = await CurrentMedia.GetPlayer();
        Projector?.SetMedia(CurrentMediaPlayer);
        if (CurrentMediaPlayer.AudioPlayer != null)
        {
            CurrentMediaPlayer.AudioPlayer.SetVolume(MediaConfig.DefaultMediaVolume);
            MediaConfig.DefaultMediaVolumeChanged += (_, volume) => CurrentMediaPlayer.AudioPlayer.SetVolume(volume);
            PlayAudio();
        }
    }

    /// <summary>
    /// Used to stop the current media playback on the client without affecting the server.
    /// This is used when the client leaves a theater zone.
    /// </summary>
    [ClientRpc]
    internal void ClientStopMedia()
    {
        CurrentMediaPlayer?.Stop();
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
