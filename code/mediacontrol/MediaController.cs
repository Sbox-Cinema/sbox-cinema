using System.Threading.Tasks;
using Sandbox;
using CinemaTeam.Plugins.Video;

namespace Cinema;

public partial class MediaController : EntityComponent<CinemaZone>
{
    public CinemaZone Zone => Entity;
    public ProjectorEntity Projector => Zone.ProjectorEntity;

    [Net, Change]
    public MediaRequest CurrentMedia { get; set; }
    private IVideoPlayer CurrentVideoPlayer { get; set; }
    [Net]
    public TimeSince StartedPlaying { get; protected set; }

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
    }

    public async void OnCurrentMediaChanged(MediaRequest oldValue, MediaRequest newValue)
    {
        await PlayCurrentMedia();
    }

    [ClientRpc]
    protected async Task PlayCurrentMedia()
    {
        if (!Game.IsClient)
            return;

        Log.Info("Request is null: " + (CurrentMedia == null));

        CurrentVideoPlayer?.Stop();
        Projector?.StopOverheadAudio();

        if (CurrentMedia == null)
            return;

        CurrentVideoPlayer = await CurrentMedia.GetPlayer();
        Projector?.SetMedia(CurrentVideoPlayer);
        // Once we have the ability to play each audio channel at a different position in the world,
        // we should branch here depending on whether the current CinemaZone has speakers.
        // For now, the sound just plays somewhere over the audience's heads.
        Projector?.PlayOverheadAudio();
    }

}
