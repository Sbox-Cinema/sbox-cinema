using System.Collections.Generic;
using System.Linq;
using Sandbox;

namespace Cinema;

public partial class MediaQueue<T> : EntityComponent where T : BaseNetworkable
{
    public class ScoredItem : BaseNetworkable
    {
        public T Item { get; set; }
        public int Score { get; set; }
    }

    public class Vote : BaseNetworkable
    {
        public T Item { get; set; }
        public IClient Voter { get; set; }
        public bool Upvote { get; set; }
    }

    [Net]
    private IList<ScoredItem> PriorityScores { get; set; }
    [Net]
    private IList<ScoredItem> FeedbackScores { get; set; }
    [Net]
    private IList<Vote> PriorityVotes { get; set; }
    [Net]
    private IList<Vote> FeedbackVotes { get; set; }
    
    public void Add(T item, int score)
    {
        PriorityScores.Add(new ScoredItem() { Item = item, Score = score });
    }

    public void ModifyPriorityScore(T item, int delta)
    {
        PriorityScores
            .First(i => i.Item == item)
            .Score += delta;
    }

    public void ModifyFeedbackScore(T item, int delta)
    {
        FeedbackScores
            .First(i => i.Item == item)
            .Score += delta;
    }

    public void Remove(T item)
    {
        var matches = PriorityScores.Where(i => i.Item == item).ToList();
        foreach(var scoredItem in matches)
        {
            PriorityScores.Remove(scoredItem);
        }
    }

    public void AddPriorityVote(T item, IClient voter, bool upvote)
    {
        PriorityVotes.Add(new Vote() { Item = item, Voter = voter, Upvote = upvote });
    }

    public void RemovePriorityVote(T item, IClient voter)
    {
        var matches = PriorityVotes.Where(v => v.Item == item && v.Voter == voter).ToList();
        foreach(var vote in matches)
        {
            PriorityVotes.Remove(vote);
        }
    }

    public void AddFeedbackVote(T item, IClient voter, bool upvote)
    {
        FeedbackVotes.Add(new Vote() { Item = item, Voter = voter, Upvote = upvote });
    }

    public void RemoveFeedbackVote(T item, IClient voter)
    {
        var matches = FeedbackVotes.Where(v => v.Item == item && v.Voter == voter).ToList();
        foreach(var vote in matches)
        {
            FeedbackVotes.Remove(vote);
        }
    }
}
