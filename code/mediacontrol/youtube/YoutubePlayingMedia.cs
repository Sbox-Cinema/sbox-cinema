using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema;

public class PlayingYouTubeMedia : PlayingMedia
{
    public override string Url => $"https://cinema-api.fly.dev/player.html?dt={VideoId}&vol=100&st={TimeSinceStartedPlaying.Relative.FloorToInt()}";
    public string VideoId { get; set; }
    public TimeSince TimeSinceStartedPlaying { get; set; }
    public int Nonce { get; set; }

    public override bool IsEqual(PlayingMedia other)
    {
        if (other is not PlayingYouTubeMedia otherYouTube) return false;
        return otherYouTube.VideoId == VideoId && otherYouTube.Nonce == Nonce;
    }
}
