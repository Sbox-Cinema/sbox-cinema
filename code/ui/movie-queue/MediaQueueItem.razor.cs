using CinemaTeam.Plugins.Media;
using Sandbox;
using Sandbox.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.UI;

public partial class MediaQueueItem : Panel
{
    public MediaQueue.ScoredItem ScoredItem { get; set; }
    public MediaInfo MediaInfo => ScoredItem?.Item?.GenericInfo;
    public MediaQueue Queue { get; set; }
    public int Priority { get; set; }

    protected override int BuildHash()
    {
        var hashCode = Priority.GetHashCode();
        foreach (var vote in ScoredItem.PriorityVotes)
        {
            var voteHash = HashCode.Combine(vote.Key.GetHashCode(), vote.Value.GetHashCode());
            hashCode = HashCode.Combine(hashCode, voteHash);
        }
        return hashCode;
    }

    public string GetUpvoteClass() => CanUpvote() ? "" : "disabled";
    public string GetDownvoteClass() => CanDownvote() ? "" : "disabled";

    public bool CanUpvote()
        => Queue.CanAddPriorityVote(ScoredItem, Game.LocalClient, true);

    public bool CanDownvote()
        => Queue.CanAddPriorityVote(ScoredItem, Game.LocalClient, false);

    public bool CanRemove()
        => Queue.CanRemove(ScoredItem, Game.LocalClient);

    public void OnVote(bool isUpvote)
    {
        Queue.AddPriorityVote(ScoredItem, Game.LocalClient, isUpvote);
    }

    public void OnRemove()
    {
        Queue.RemoveItem(ScoredItem, Game.LocalClient);
    }
}
