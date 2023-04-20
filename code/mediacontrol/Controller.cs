using System.Collections.Generic;
using System.Linq;
using Sandbox;

namespace Cinema;

public partial class MediaController : Entity
{
    [Net]
    public ProjectorEntity Projector { get; set; }

    [Net]
    public IList<Media> Queue { get; set; } = new List<Media>();

    public Media CurrentMedia => Queue.FirstOrDefault();

    public Media Next => Queue.ElementAtOrDefault(1);

    public MediaController()
    {
    }

    [Event.Tick.Server]
    public void ServerUpdate()
    {
        // @TODO: Figure out how to tell if the media is done playing and goto next if needed
    }

    [Event.Tick.Client]
    public void ClientUpdate()
    {
        PlayCurrentMediaOnProjector();
    }

    public void AddToQueue(Media movie)
    {
        Queue.Add(movie);
    }

    public void StartNext()
    {
        Queue.RemoveAt(0);
        PlayCurrentMediaOnProjector();
    }

    protected void PlayCurrentMediaOnProjector()
    {
        if ( CurrentMedia == null ) return;

        if ( Game.IsServer )
        {
            // Force the url on the client to change via RPC
            ClientSetMediaSourceUrl(CurrentMedia.Url);
        }
        else
        {
            if ( Projector.ProjectionImage.MediaSource.CurrentUrl != CurrentMedia.Url )
                Projector.ProjectionImage.MediaSource.SetUrl(CurrentMedia.Url);
        }
    }

    [ClientRpc]
    private void ClientSetMediaSourceUrl(string url)
    {
        Projector.ProjectionImage.MediaSource.SetUrl(url);
    }

    [ConCmd.Server("queue")]
    public static void ConsoleAddMedia(string url)
    {
        foreach ( var controller in Entity.All.OfType<MediaController>() )
            controller.AddToQueue(new Media() { Url = url, Requestor = ConsoleSystem.Caller });
    }

    [ConCmd.Server("skip")]
    public static void ConsoleSkip()
    {
        foreach ( var controller in Entity.All.OfType<MediaController>() )
            controller.StartNext();
    }
}
