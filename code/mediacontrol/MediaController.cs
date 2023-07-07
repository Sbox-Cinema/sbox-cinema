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
public partial class MediaController : EntityComponent<CinemaZone>
{
    public CinemaZone Zone => Entity;
    public ProjectorEntity Projector => Zone.ProjectorEntity;

    [Net, Change]
    public MediaRequest CurrentMedia { get; set; }
    private IVideoPlayer CurrentVideoPlayer { get; set; }
    [Net]
    public TimeSince StartedPlaying { get; private set; }
    [Net]
    public bool IsPaused { get; set; }

    public event EventHandler StartPlaying;
    public event EventHandler StopPlaying;

    [GameEvent.Entity.PostSpawn]
    private void OnPostSpawn()
    {
        StartPlaying += (_,_) => Zone.SetLightsEnabled(false);
        StopPlaying += (_,_) => Zone.SetLightsEnabled(true);
    }

    [GameEvent.Tick.Server]
    private void OnTick()
    {
        if (CurrentMedia != null && StartedPlaying > CurrentMedia.GenericInfo.Duration)
        {
            StopMedia(null);
        }
    }

    private static (MediaController controller, IClient client) GetControllerAndClient(int zoneId, int clientId)
    {
        var zone = Sandbox.Entity.FindByIndex(zoneId) as CinemaZone;
        var controller = zone.MediaController;
        IClient client = null;
        if (clientId > 0)
        {
            client = Sandbox.Entity.FindByIndex(clientId) as IClient;
        }
        return (controller, client);
    }

    [ConCmd.Server]
    public async static void PlayMedia(int zoneId, int clientId, int providerId, string request)
    {
        var (controller, client) = GetControllerAndClient(zoneId, clientId);
        await controller.PlayMedia(client, providerId, request);
    }
    public async Task PlayMedia(IClient client, int providerId, string request)
    {
        var provider = VideoProviderManager.Instance[providerId];
        CurrentMedia = await provider.CreateRequest(client, request);
        StartedPlaying = 0;
        IsPaused = false;
        StartPlaying?.Invoke(this, null);
    }

    [ConCmd.Server]
    public static void TogglePauseMedia(int zoneId, int clientId)
    {
        var (controller, client) = GetControllerAndClient(zoneId, clientId);
        controller.TogglePauseMedia(client);
    }

    public void TogglePauseMedia(IClient client)
    {

        // TODO: Return early if the client is not allowed to pause the media.
        IsPaused = !IsPaused;
        SetPauseMedia(IsPaused);
    }

    [ClientRpc]
    public void SetPauseMedia(bool shouldPause)
    {
        CurrentVideoPlayer.SetPaused(shouldPause);
    }

    [ConCmd.Server]
    public static void StopMedia(int zoneId, int clientId)
    {
        var (controller, client) = GetControllerAndClient(zoneId, clientId);
        controller.StopMedia(client);
    }

    public void StopMedia(IClient client)
    {
        CurrentMedia = null;
        StopPlaying?.Invoke(this, null);
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
        Projector?.SetMedia(CurrentVideoPlayer);
        PlayAudio();
    }

    private void PlayAudio()
    {
        var centerChannel = CinemaZone.AudioChannel.Center;
        // For now, we only support playing the center channel, but once VideoPlayer supports
        // multiple audio channels,
        if (Zone.HasSpeaker(centerChannel))
        {
            CurrentVideoPlayer.PlayAudio(Zone.GetSpeaker(centerChannel));
        }
        else
        {
            // Once we have the ability to play each audio channel at a different position in the world,
            // we should branch here depending on whether the current CinemaZone has speakers.
            // For now, the sound just plays somewhere over the audience's heads.
            Projector?.PlayOverheadAudio();
        }
    }

    private void StopAudio()
    {
        Zone.StopAllSpeakerAudio();
        Projector?.StopOverheadAudio();
    }

}
