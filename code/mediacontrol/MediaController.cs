using System;
using System.Collections.Generic;
using System.Linq;
using CinemaTeam.Plugins.Video;
using Sandbox;

namespace Cinema;

/// <summary>
/// Manages the media request queue. <br/>
/// Stores the "waiting image". <br/>
/// Awards money to players whose media is liked when played. <br/>
/// Tells the projector to play the next media.
/// </summary>
public partial class MediaController : EntityComponent<CinemaZone>, ISingletonComponent
{
    public CinemaZone Zone => Entity;
    public ProjectorEntity Projector => Zone?.ProjectorEntity;

    public List<Media> RequestQueue { get; set; } = new List<Media>();

    [Net]
    public IList<Media> Queue { get; set; }

    public static string WaitingImage => "https://i.pinimg.com/originals/62/c7/c2/62c7c28439ff95418a16b0d0c907fa18.jpg";
    public static int MoneyPerLikePerMinute => 30;

    public Media NextMedia => Queue.OrderBy(m => m.ListScore).FirstOrDefault();
    [Net, Change]
    public MediaRequest CurrentMedia { get; set; }

    public IVideoPlayer CurrentVideoPlayer { get; set; }

    [Net]
    public TimeSince TimeSinceStartedPlaying { get; set; }

    [GameEvent.Tick.Server]
    public void ServerUpdate()
    {
        while (RequestQueue.Count > 0)
        {
            var request = RequestQueue.First();
            RequestQueue.RemoveAt(0);
            request.ListScore = Queue.Count;
            Queue.Add(request);
        }

        if (CurrentMedia == null)
        {
            var media = new MediaRequest();
            media["Url"] = WaitingImage;
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

        var media = await YouTube.CreateFromRequest(movie);
        RequestQueue.Add(media);
    }

    public void OnCurrentMediaChanged(MediaRequest oldValue, MediaRequest newValue)
    {
        // Log the media change, including URLs.
        Log.Info($"{Name} - Media changed from {oldValue["Url"]} to {newValue["Url"]}.");
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
        var next = NextMedia;
        if (Queue.Count > 0)
            Queue.RemoveAt(0);
        // StartPlayingMedia(next);
    }

    public void RemoveMediaAtIndex(int index, IClient remover)
    {
        var media = Queue.ElementAtOrDefault(index);
        if (media is null) return;

        if (media.Requestor != remover) return;

        Queue.RemoveAt(index);
    }

    protected void PlayCurrentMediaOnProjector(bool forceUpdate = false)
    {
        if (CurrentMedia != null && CurrentVideoPlayer == null)
        {
            SetWaitingImage();
            return;
        }

        // PlayYouTubeVideo(CurrentMedia.YouTubeId, CurrentMedia.Nonce, TimeSinceStartedPlaying, forceUpdate);
    }

    private void OnFinishedPlayingMedia(Media media)
    {
        var totalLikes = media.Likes.Where(c => c != media.Requestor).Count() - media.Dislikes.Where(c => c != media.Requestor).Count();
        if (totalLikes <= 0) return;
        if (media.Requestor?.Pawn is not Player player) return;

        var timePlayed = TimeSinceStartedPlaying.Relative;
        var minutesPlayed = (int)Math.Max(1, Math.Floor(timePlayed / 60));
        var moneyEarned = totalLikes * minutesPlayed * MoneyPerLikePerMinute;

        Log.Info($"Awarded {media.Requestor.Name} ${moneyEarned} for playing {media.Title} and getting {totalLikes} likes.");
        player.AddMoney(moneyEarned);
    }

    private void StartPlayingMedia(IVideoPlayer media)
    {
        if (CurrentVideoPlayer != null)
        {
            // OnFinishedPlayingMedia(CurrentMedia);
        }

        // new media is playing
        CurrentVideoPlayer = media;
        TimeSinceStartedPlaying = 0;
        PlayCurrentMediaOnProjector(true);
        return;
    }

    private void PlayNextMediaIfReady()
    {
        if (CurrentVideoPlayer == null)
        {
            if (NextMedia != null)
            {
                StartNext();
            }

            return;
        }

        //if (TimeSinceStartedPlaying > CurrentMedia.Duration + 1)
        //{
        //    StartNext();
        //}

    }

    [ClientRpc]
    private void ClientSetWaitingImage() => SetWaitingImage();

    public void SetWaitingImage()
    {
        if (Game.IsServer)
        {
            ClientSetWaitingImage();
            return;
        }

        if (CurrentMedia == null)
            return;

        Log.Info("Creating new web surface player");

        CurrentVideoPlayer = new WebSurfaceVideoPlayer(CurrentMedia);

        Projector.SetMedia(CurrentVideoPlayer);
    }

    private void PlayYouTubeVideo(string youtubeId, int nonce, float timeSinceStarted, bool forceUpdate = true)
    {
        if (Game.IsServer)
        {
            ClientPlayYouTubeVideo(youtubeId, nonce, timeSinceStarted, forceUpdate);
            return;
        }

        var media = new PlayingYouTubeMedia()
        {
            VideoId = youtubeId,
            TimeSinceStartedPlaying = timeSinceStarted,
            Nonce = nonce
        };

        // Projector.SetMedia(media);
    }

    [ClientRpc]
    private void ClientPlayYouTubeVideo(string url, int nonce, float timeSinceStarted, bool forceUpdate = true)
    {
        PlayYouTubeVideo(url, nonce, timeSinceStarted, forceUpdate);
    }



    private void DebugQueueToLog()
    {
        Log.Info($"Debug {Projector.Name}");
        foreach (var media in Queue.OrderBy(m => m.ListScore))
        {
            Log.Info($"({media.ListScore}) {media.Title}");
        }
    }

    [ConCmd.Client("controller.debug.queue.client")]
    public static void DebugQueueClient()
    {
        var controller = Sandbox.Entity.All.OfType<ProjectorEntity>().OrderBy(e => e.Position.DistanceSquared(Game.LocalPawn.Position)).FirstOrDefault()?.Components.Get<MediaController>();

        if (controller is null) return;
        controller.DebugQueueToLog();
    }

    [ConCmd.Server("controller.debug.queue.server")]
    public static void DebugQueueServer()
    {
        var controller = Sandbox.Entity.All.OfType<ProjectorEntity>().OrderBy(e => e.Position.DistanceSquared(ConsoleSystem.Caller.Pawn.Position)).FirstOrDefault()?.Components.Get<MediaController>();

        if (controller is null) return;
        controller.DebugQueueToLog();
    }

    [ConCmd.Server]
    public static void RequestAddMedia(int projectorId, string youtubeId)
    {
        var projector = Sandbox.Entity.FindByIndex(projectorId);
        var controller = projector?.Components.Get<MediaController>();
        if (controller is null) return;

        var request = new MediaRequest() { Requestor = ConsoleSystem.Caller };
        request[YouTube.RequestData.YouTubeId] = youtubeId;

        controller.RequestMedia(request);
    }

    [ConCmd.Server]
    public static void Skip(int projectorId)
    {
        var projector = Sandbox.Entity.FindByIndex(projectorId);
        var controller = projector?.Components.Get<MediaController>();
        if (controller is null) return;

        controller.StartNext();
    }

    [ConCmd.Server]
    public static void RemoveMedia(int projectorId, int index)
    {
        var projector = Sandbox.Entity.FindByIndex(projectorId);
        var controller = projector?.Components.Get<MediaController>();
        if (controller is null) return;

        controller.RemoveMediaAtIndex(index, ConsoleSystem.Caller);
    }

    [ConCmd.Server]
    public static void VoteForMedia(int projectorId, int nonce, bool upvote)
    {
        var projector = Sandbox.Entity.FindByIndex(projectorId);
        var controller = projector?.Components.Get<MediaController>();

        if (controller is null) return;

        var media = controller.Queue.Where(m => m.Nonce == nonce).FirstOrDefault();
        if (media is null) return;

        media.VoteFor(ConsoleSystem.Caller, upvote);
    }

    [ConCmd.Server]
    public static void GiveMediaLike(int projectorId, int nonce, bool like)
    {
        var projector = Sandbox.Entity.FindByIndex(projectorId);
        var controller = projector?.Components.Get<MediaController>();

        if (controller is null) return;

        var media = controller.CurrentVideoPlayer;
        // if (media?.Nonce != nonce) return;

        // media.GiveLike(ConsoleSystem.Caller, like);
    }
}
