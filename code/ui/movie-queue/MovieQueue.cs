using System.Linq;
using System;
using System.Collections.Generic;
using Sandbox;
using Sandbox.UI;
using System.Collections;

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

    public Media NowPlaying => Controller?.PlayingMedia ?? null;

    public TimeSince TimeSinceStartedPlaying => Controller?.TimeSinceStartedPlaying ?? 0;

    public string NowPlayingTimeString => NowPlaying == null ? "0:00 / 0:00" : $"{TimeSpan.FromSeconds(TimeSinceStartedPlaying.Relative):hh\\:mm\\:ss} / {TimeSpan.FromSeconds(NowPlaying.Duration):hh\\:mm\\:ss}";

    public TextEntry MovieIDEntry { get; set; }

    public bool Open()
    {
        var localPawn = Game.LocalPawn;
        var closestProjector = Entity
            .All
            .OfType<ProjectorEntity>()
            .OrderBy(x => x.Position.Distance(localPawn.Position))
            .FirstOrDefault();

        if (closestProjector == null)
            return false;

        Controller = closestProjector.Controller;
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
        videoId = ParseYouTubeIdIfUrl(videoId);
        if (string.IsNullOrWhiteSpace(videoId))
            return;
        MovieIDEntry.Text = "";
        MovieIDEntry.Disabled = true;
        MovieIDEntry.Disabled = false;

        var valid = await Media.VerifyYouTubeId(videoId);

        if (!valid)
        {
            Log.Error("Invalid YouTube ID");
            return;
        }

        Controller.RequestMedia(videoId);
    }

    protected string ParseYouTubeIdIfUrl(string userInput)
    {
        var possibleUrl = userInput;

        // If this is clearly a website but missing the URI scheme, add it here.
        if (userInput.StartsWith("youtu") || userInput.StartsWith("www.youtu"))
        {
            possibleUrl = "https://" + userInput;
        }

        // If we can't make URI from this input, just return the input as-is since it's probably an ID.
        if (!Uri.TryCreate(possibleUrl, UriKind.Absolute, out Uri uri))
        {
            return userInput;
        }
        if (uri.Scheme != Uri.UriSchemeHttps && uri.Scheme != Uri.UriSchemeHttp)
        {
            return userInput;
        }

        // Youtu.be doesn't use a query string and the video ID is just the last part of the path.
        if (uri.Host.Contains("youtu.be"))
        {
            return System.IO.Path.GetFileNameWithoutExtension(uri.LocalPath);
        }
        // If this is just some other website, return the input as-is.
        if (!uri.Host.Contains("youtube"))
        {
            return userInput;
        }

        var queryString = System.Web.HttpUtility.ParseQueryString(uri.Query);
        var videoIdParam = queryString["v"];
        if (string.IsNullOrEmpty(videoIdParam))
        {
            Log.Error("No video ID in URL");
            return null;
        }
        return videoIdParam;
    }

    protected void OnSkip()
    {
        Controller.Skip();
    }

    protected void OnClose()
    {
        IsOpen = false;
        Controller = null;
    }
}
