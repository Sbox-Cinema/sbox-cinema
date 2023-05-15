using System;
using System.Collections.Generic;
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

    public MediaController Controller { get; set; } = null;

    public IList<Media> Queue => Controller?.Queue ?? new List<Media>();

    public Media NowPlaying => Controller?.CurrentMedia ?? null;

    public TextEntry MovieIDEntry { get; set; }

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

        return HashCode.Combine(Visible, NowPlaying, queueHash);
    }


    protected void OnQueue()
    {
        var videoId = MovieIDEntry.Text;
        if (videoId == "")
            return;
        var fullUrl = $"https://www.youtube.com/embed/{videoId}?autoplay=1;frameborder=0";
        Controller.AddToQueue(fullUrl);
        MovieIDEntry.Text = "";
        MovieIDEntry.Disabled = true;
        MovieIDEntry.Disabled = false;
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
