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
    public TimeSince StartedPlaying { get; protected set; }

    public event EventHandler StartPlaying;
    public event EventHandler StopPlaying;

    [GameEvent.Entity.PostSpawn]
    public void OnPostSpawn()
    {
        StartPlaying += (_,_) => Zone.SetLightsEnabled(false);
        StopPlaying += (_,_) => Zone.SetLightsEnabled(true);
    }

    [ConCmd.Server]
    public async static void RequestMedia(int zoneId, int clientId, int providerId, string request)
    {
        var zone = Sandbox.Entity.FindByIndex(zoneId) as CinemaZone;
        var controller = zone.MediaController;
        IClient client = null;
        if (clientId > 0)
        {
            client = Sandbox.Entity.FindByIndex(clientId) as IClient;
        }
        var provider = VideoProviderManager.Instance[providerId];
        controller.CurrentMedia = await provider.CreateRequest(client, request);
        controller.StartPlaying?.Invoke(controller, null);
    }

    [ConCmd.Server]
    public static void StopMedia(int zoneId, int clientId)
    {
        var zone = Sandbox.Entity.FindByIndex(zoneId) as CinemaZone;
        var controller = zone.MediaController;
        IClient client = null;
        if (clientId > 0)
        {
            client = Sandbox.Entity.FindByIndex(clientId) as IClient;
        }
        controller.CurrentMedia = null;
        controller.StopPlaying?.Invoke(controller, null);
    }

    public async void OnCurrentMediaChanged(MediaRequest oldValue, MediaRequest newValue)
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
    protected async Task PlayCurrentMedia()
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

    protected void PlayAudio()
    {
        var centerChannel = CinemaZone.SpeakerChannel.Center;
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

    protected void StopAudio()
    {
        Zone.StopAllSpeakerAudio();
        Projector?.StopOverheadAudio();
    }

}
