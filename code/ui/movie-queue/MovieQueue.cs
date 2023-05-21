using System;
using System.Collections.Generic;
using Sandbox;
using Sandbox.UI;

namespace Cinema.UI;

public partial class MovieQueue : Panel
{
    public static MovieQueue Instance { get; set; }

    public MovieQueue()
    {
        Instance = this;
    }

    public bool Visible { get; set; } = false;

    public string VisibleClass => Visible ? "visible" : "";

    public MediaController Controller { get; set; } = null;

    public IList<Media> Queue => Controller?.Queue ?? new List<Media>();

    public Media NowPlaying => Controller?.PlayingMedia ?? null;

    public TimeSince TimeSinceStartedPlaying => Controller?.TimeSinceStartedPlaying ?? 0;

    public string NowPlayingTimeString => NowPlaying == null ? "0:00 / 0:00" : $"{TimeSpan.FromSeconds(TimeSinceStartedPlaying.Relative).ToString(@"hh\:mm\:ss")} / {TimeSpan.FromSeconds(NowPlaying.Duration).ToString(@"hh\:mm\:ss")}";

    public TextEntry MovieIDEntry { get; set; }

    protected override int BuildHash()
    {
        var queueHash = 11;

        if (Queue != null)
        {
            foreach (var media in Queue)
            {
                if (media == null)
                {
                    Log.Info($"Null media");
                    continue;
                }
                queueHash = queueHash * 31 + media.GetHashCode();
            }
        }

        return HashCode.Combine(Visible, NowPlaying, queueHash, NowPlayingTimeString);
    }


    protected async void OnQueue()
    {
        var videoId = MovieIDEntry.Text;
        if (videoId == "")
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

    protected void OnSkip()
    {
        Controller.Skip();
    }

    protected void OnClose()
    {
        Visible = false;
        Controller = null;
    }
}
