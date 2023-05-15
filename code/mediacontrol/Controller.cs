using System.Collections.Generic;
using System.Linq;
using Sandbox;

namespace Cinema;

public partial class MediaController : EntityComponent<ProjectorEntity>, ISingletonComponent
{
    public ProjectorEntity Projector => Entity;

    [Net]
    public IList<Media> Queue { get; set; } = new List<Media>();

    public static string StaticImage => "https://i.pinimg.com/originals/62/c7/c2/62c7c28439ff95418a16b0d0c907fa18.jpg";

    public Media CurrentMedia => Queue.FirstOrDefault();

    public Media Next => Queue.ElementAtOrDefault(1);

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

    public void AddToQueue(Media movie)
    {
        Queue.Add(movie);
        if (Queue.Count == 1)
            PlayCurrentMediaOnProjector(true);
    }

    public void AddToQueue(string url)
    {
        if (Game.IsServer) return;
        AddMedia(Entity.NetworkIdent, url);
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

        SetMediaSourceUrl(CurrentMedia.Url, forceUpdate);
    }

    private void SetMediaSourceUrl(string url, bool forceUpdate = true)
    {
        if (Game.IsServer)
        {
            ClientSetMediaSourceUrl(url, forceUpdate);
            return;
        }

        if (forceUpdate || Projector.CurrentUrl != url)
        {
            Projector.CurrentUrl = url;
        }
    }

    [ClientRpc]
    private void ClientSetMediaSourceUrl(string url, bool forceUpdate = true)
    {
        SetMediaSourceUrl(url, forceUpdate);
    }

    //Hacky, @todo: make this cleaner and target specific controllers

    [ConCmd.Server("queue")]
    public static void AddMedia(int projectorId, string url)
    {
        var projector = Sandbox.Entity.FindByIndex(projectorId);
        var controller = projector?.Components.Get<MediaController>();
        if (controller is null) return;

        controller.AddToQueue(new Media() { Url = url, Requestor = ConsoleSystem.Caller });
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
