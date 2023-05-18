using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sandbox;

namespace Cinema;

public partial class MediaController : EntityComponent<ProjectorEntity>, ISingletonComponent
{
    public ProjectorEntity Projector => Entity;

    [Net]
    public IList<Media> Queue { get; set; }

    public static string StaticImage => "https://i.pinimg.com/originals/62/c7/c2/62c7c28439ff95418a16b0d0c907fa18.jpg";

    public Media CurrentMedia => Queue.FirstOrDefault();

    [GameEvent.Tick.Server]
    public void ServerUpdate()
    {
        PlayCurrentMediaOnProjector();
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

        Queue.Add(media);
        if (Queue.Count == 1)
            PlayCurrentMediaOnProjector(true);
    }

    public void RequestMedia(string youTubeId)
    {
        if (Game.IsServer) return;
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
        Queue.RemoveAt(0);
        PlayCurrentMediaOnProjector(true);
    }

    protected void PlayCurrentMediaOnProjector(bool forceUpdate = false)
    {
        if (CurrentMedia == null)
        {
            SetMediaSourceUrl(StaticImage);
            return;
        }

        if (CurrentMedia.YouTubeId == null)
        {
            return;
        }

        SetMediaSourceUrl(CurrentMedia.YouTubeId, forceUpdate);
    }

    private void SetMediaSourceUrl(string youtubeId, bool forceUpdate = true)
    {
        if (Game.IsServer)
        {
            ClientSetMediaSourceUrl(youtubeId, forceUpdate);
            return;
        }

        if (youtubeId.Substring(0, 4) == "http")
        {
            Projector.SetStaticImage(youtubeId);
            return;
        }

        if (forceUpdate || Projector.CurrentUrl != youtubeId)
        {
            Projector.PlayYouTubeVideo(youtubeId);
        }
    }

    [ClientRpc]
    private void ClientSetMediaSourceUrl(string url, bool forceUpdate = true)
    {
        SetMediaSourceUrl(url, forceUpdate);
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
