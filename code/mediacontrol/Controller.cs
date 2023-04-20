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

    public static string StaticImage => "https://i.pinimg.com/originals/62/c7/c2/62c7c28439ff95418a16b0d0c907fa18.jpg";

    public Media CurrentMedia => Queue.FirstOrDefault();

    public Media Next => Queue.ElementAtOrDefault(1);

    public MediaController()
    {
    }

    [Event.Tick.Server]
    public void ServerUpdate()
    {
        PlayCurrentMediaOnProjector();
    }

    [Event.Tick.Client]
    public void ClientUpdate()
    {
        PlayCurrentMediaOnProjector();
    }

    public void AddToQueue(Media movie)
    {
        Queue.Add(movie);
        if ( Queue.Count == 1 )
            PlayCurrentMediaOnProjector(true);
    }

    public void StartNext()
    {
        Queue.RemoveAt(0);
        PlayCurrentMediaOnProjector(true);
    }

    protected void PlayCurrentMediaOnProjector(bool forceUpdate = false)
    {
        if ( CurrentMedia == null )
        {
            SetMediaSourceUrl(StaticImage);
            return;
        }

        SetMediaSourceUrl(CurrentMedia.Url, forceUpdate);
    }

    private void SetMediaSourceUrl(string url, bool forceUpdate = true)
    {
        if ( Game.IsServer )
        {
            ClientSetMediaSourceUrl(url, forceUpdate);
            return;
        }

        if ( forceUpdate || Projector.ProjectionImage.MediaSource.CurrentUrl != url )
        {
            Projector.ProjectionImage.MediaSource.SetUrl(url);
        }
    }

    [ClientRpc]
    private void ClientSetMediaSourceUrl(string url, bool forceUpdate = true)
    {
        SetMediaSourceUrl(url, forceUpdate);
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
