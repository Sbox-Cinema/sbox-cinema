using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sandbox;

namespace Cinema;

public partial class MediaController : EntityComponent<ProjectorEntity>, ISingletonComponent
{
    public ProjectorEntity Projector => Entity;

    public List<Media> RequestQueue { get; set; } = new List<Media>();

    [Net]
    public IList<Media> Queue { get; set; }

    public static string WaitingImage => "https://i.pinimg.com/originals/62/c7/c2/62c7c28439ff95418a16b0d0c907fa18.jpg";

    public Media NextMedia => Queue.FirstOrDefault();

    [Net]
    public Media PlayingMedia { get; set; }
    public TimeSince TimeSinceStartedPlaying { get; set; }

    [GameEvent.Tick.Server]
    public void ServerUpdate()
    {
        while (RequestQueue.Count > 0)
        {
            var request = RequestQueue.First();
            RequestQueue.RemoveAt(0);
            Queue.Add(request);
        }

        PlayNextMediaIfReady();
    }

    [GameEvent.Tick.Client]
    public void ClientUpdate()
    {
        PlayCurrentMediaOnProjector();
    }

    public async void RequestMedia(MediaRequest movie)
    {
        if (Game.IsClient)
        {
            Log.Error("Cannot add to queue on client");
            return;
        }

        var media = await Media.CreateFromRequest(movie);
        Log.Info($"Media request: {media}");
        RequestQueue.Add(media);
    }

    public void RequestMedia(string youTubeId)
    {
        if (Game.IsServer) return;
        if (!Entity.IsValid())
        {
            Log.Error($"Tried to request on an invalid controller.");
            return;
        }

        RequestAddMedia(Entity.NetworkIdent, youTubeId);
    }

    public void Skip()
    {
        if (Game.IsServer)
        {
            StartNext();
            return;
        }

        Skip(Entity.NetworkIdent);
    }

    public void StartNext()
    {
        Log.Info("Playing next media");
        var next = NextMedia;
        if (Queue.Count > 0)
            Queue.RemoveAt(0);
        StartPlayingMedia(next);
    }

    protected void PlayCurrentMediaOnProjector(bool forceUpdate = false)
    {
        if (PlayingMedia?.YouTubeId == null)
        {
            SetWaitingImage();
            return;
        }

        PlayYouTubeVideo(PlayingMedia.YouTubeId, forceUpdate);
    }

    private void StartPlayingMedia(Media media)
    {
        // new media is playing
        PlayingMedia = media;
        TimeSinceStartedPlaying = 0;
        PlayCurrentMediaOnProjector(true);
        return;
    }

    private void PlayNextMediaIfReady()
    {
        if (PlayingMedia == null)
        {
            if (NextMedia != null)
            {
                StartNext();
            }

            return;
        }

        if (TimeSinceStartedPlaying > PlayingMedia.Duration + 1)
        {
            StartNext();
        }

    }

    private void SetWaitingImage()
    {
        if (Game.IsServer)
        {
            ClientSetWaitingImage();
            return;
        }

        Projector.SetStaticImage(WaitingImage);
    }

    private void PlayYouTubeVideo(string youtubeId, bool forceUpdate = true)
    {
        if (Game.IsServer)
        {
            ClientPlayYouTubeVideo(youtubeId, forceUpdate);
            return;
        }

        if (Projector.CurrentVideoId == youtubeId && !forceUpdate)
            return;

        Projector.PlayYouTubeVideo(youtubeId);
    }

    [ClientRpc]
    private void ClientPlayYouTubeVideo(string url, bool forceUpdate = true)
    {
        PlayYouTubeVideo(url, forceUpdate);
    }

    [ClientRpc]
    private void ClientSetWaitingImage()
    {
        SetWaitingImage();
    }


    //Hacky, @todo: make this cleaner and target specific controllers

    [ConCmd.Server("queue")]
    public static void RequestAddMedia(int projectorId, string youtubeId)
    {
        var projector = Sandbox.Entity.FindByIndex(projectorId);
        var controller = projector?.Components.Get<MediaController>();
        if (controller is null) return;

        controller.RequestMedia(new MediaRequest()
        {
            YouTubeId = youtubeId,
            Requestor = ConsoleSystem.Caller
        });
    }

    [ConCmd.Server("skip")]
    public static void Skip(int projectorId)
    {
        var projector = Sandbox.Entity.FindByIndex(projectorId);
        var controller = projector?.Components.Get<MediaController>();
        if (controller is null) return;

        controller.StartNext();
    }
}
