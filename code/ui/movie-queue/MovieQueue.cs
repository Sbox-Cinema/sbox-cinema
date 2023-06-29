using System.Linq;
using System;
using System.Collections.Generic;
using Sandbox;
using Sandbox.UI;
using CinemaTeam.Plugins.Video;

namespace Cinema.UI;

public partial class MovieQueue : Panel, IMenuScreen
{
    public static MovieQueue Instance { get; set; }

    public string Name => "Movie Queue";
    public bool IsOpen { get; protected set; }

    public MovieQueue()
    {
        Instance = this;
    }

    public Panel Thumbnail { get; set; }

    public string VisibleClass => IsOpen ? "visible" : "";

    public MediaController Controller { get; set; } = null;

    public IList<Media> Queue => Controller?.Queue.OrderBy(m => m.ListScore).ToList() ?? new List<Media>();

    public Media NowPlaying => /* Controller?.CurrentMedia ?? */ null;

    public TimeSince TimeSinceStartedPlaying => Controller?.TimeSinceStartedPlaying ?? 0;

    public string NowPlayingTimeString => NowPlaying == null ? "0:00 / 0:00" : $"{TimeSpan.FromSeconds(TimeSinceStartedPlaying.Relative):hh\\:mm\\:ss} / {TimeSpan.FromSeconds(NowPlaying.Duration):hh\\:mm\\:ss}";

    public TextEntry MovieIDEntry { get; set; }

    public bool Open()
    {
        if (Game.LocalPawn is not Player ply)
            return false;

        var currentTheaterZone = ply.GetCurrentTheaterZone();

        // If we're not in a theater zone, don't open the queue.
        if (currentTheaterZone == null)
            return false;

        Controller = currentTheaterZone.MediaController;
        IsOpen = true;
        return true;
    }

    public void Close()
    {
        IsOpen = false;
        Controller = null;
    }

    protected static bool CanRemoveMedia(Media media)
    {
        return media.CanRemove(Game.LocalClient);
    }

    protected static bool IsPlayer(Media media)
    {
        return media.IsPlayer(Game.LocalClient);
    }

    protected static bool CanVoteFor(Media media, bool upvote)
    {
        return upvote ? media.CanUpVote(Game.LocalClient) : media.CanDownVote(Game.LocalClient);
    }

    protected static bool CanGiveLike(Media media, bool like)
    {
        return like ? media.CanLike(Game.LocalClient) : media.CanDislike(Game.LocalClient);
    }

    protected void OnRemove(Media media)
    {
        MediaController.RemoveMedia(Controller.NetworkIdent, Controller.Queue.IndexOf(media));
    }

    protected void OnVote(Media media, bool voteFor)
    {
        if (voteFor ? !media.CanUpVote(Game.LocalClient) : !media.CanDownVote(Game.LocalClient)) return;
        MediaController.VoteForMedia(Controller.NetworkIdent, media.Nonce, voteFor);
    }

    protected void OnLike(Media media, bool like)
    {
        if (like ? !media.CanLike(Game.LocalClient) : !media.CanDislike(Game.LocalClient)) return;
        MediaController.GiveMediaLike(Controller.NetworkIdent, media.Nonce, like);
    }

    protected override int BuildHash()
    {
        var queueHash = 11;

        if (Queue != null)
        {
            foreach (var media in Queue)
            {
                queueHash = queueHash * 31 + media.GetHashCode();
            }
        }

        return HashCode.Combine(IsOpen, Queue.Count, NowPlaying, queueHash, NowPlayingTimeString);
    }

    public override void Tick()
    {
        Thumbnail?.Style.SetBackgroundImage(NowPlaying.Thumbnail);
    }

    protected async void OnQueue()
    {
        var videoId = MovieIDEntry.Text;
        videoId = YouTube.ParseVideoIdIfUrl(videoId);
        if (string.IsNullOrWhiteSpace(videoId))
            return;
        MovieIDEntry.Text = "";
        MovieIDEntry.Disabled = true;
        MovieIDEntry.Disabled = false;

        var valid = await YouTube.VerifyYouTubeId(videoId);

        if (!valid)
        {
            Log.Error("Invalid YouTube ID");
            return;
        }

        Controller.RequestMedia(videoId);
    }

    protected void OnSkip()
    {
        Controller.Skip();
    }

    protected void OnClose()
    {
        (Game.LocalPawn as Player).CloseMenu(this);
    }
}
