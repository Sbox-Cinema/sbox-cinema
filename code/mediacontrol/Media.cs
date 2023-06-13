using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Sandbox;

namespace Cinema;

public partial class Media : BaseNetworkable
{
    [Net]
    public int Nonce { get; set; }

    [Net]
    public string YouTubeId { get; set; }

    [Net]
    public IClient Requestor { get; set; }

    [Net]
    public string Title { get; set; }

    [Net]
    public int Duration { get; set; } = 0;

    public string DurationFormatted => TimeSpan.FromSeconds(Duration).ToString(@"hh\:mm\:ss");

    [Net]
    public string Thumbnail { get; set; }

    [Net]
    public bool CanEmbed { get; set; } = false;

    [Net]
    public bool Verified { get; set; } = false;

    [Net]
    public int ListScore { get; set; }

    [Net]
    public IList<IClient> VotesFor { get; set; }

    [Net]
    public IList<IClient> VotesAgainst { get; set; }

    [Net]
    public IList<IClient> Likes { get; set; }

    [Net]
    public IList<IClient> Dislikes { get; set; }

    public int TotalLikes => Likes.Count - Dislikes.Count;

    public void VoteFor(IClient voter, bool upvote)
    {
        var originalScore = ListScore;

        if (upvote)
        {
            if (VotesFor.Contains(voter)) return;
            VotesFor.Add(voter);
            var didRemove = VotesAgainst.Remove(voter);
            originalScore -= didRemove ? 2 : 1;
            ListScore = originalScore;
            return;
        }

        if (!upvote)
        {
            if (VotesAgainst.Contains(voter)) return;
            VotesAgainst.Add(voter);
            var didRemove = VotesFor.Remove(voter);
            originalScore += didRemove ? 2 : 1;
            ListScore = originalScore;
            return;
        }
    }

    public void GiveLike(IClient liker, bool like)
    {
        if (like)
        {
            if (Likes.Contains(liker)) return;
            Likes.Add(liker);
            Dislikes.Remove(liker);
            return;
        }

        if (!like)
        {
            if (Dislikes.Contains(liker)) return;
            Dislikes.Add(liker);
            Likes.Remove(liker);
            return;
        }
    }

    public bool IsPlayer(IClient client) => client == Requestor;
    public bool CanRemove(IClient client) => IsPlayer(client);
    public bool CanUpVote(IClient client) => !VotesFor.Contains(client);
    public bool CanDownVote(IClient client) => !VotesAgainst.Contains(client);
    public bool CanLike(IClient client) => !Likes.Contains(client);
    public bool CanDislike(IClient client) => !Dislikes.Contains(client);

    public override string ToString()
    {
        return $"Media: {YouTubeId} ({Title}) by {Requestor.Name}";
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(HashCode.Combine(YouTubeId, Requestor.Name, Title, Duration), HashCode.Combine(Thumbnail, CanEmbed, Verified, VotesFor.Count, VotesAgainst.Count, ListScore, Likes.Count, Dislikes.Count));
    }



    private static int NonceCounter = 0;

    public static async Task<Media> CreateFromRequest(MediaRequest request)
    {
        ParseApiResponse response;

        try
        {
            response = await Http.RequestJsonAsync<ParseApiResponse>($"{CinemaApi.Url}/api/parse2?type=yt&id={request.RequestData}", "GET");
        }
        catch (Exception e)
        {
            Log.Error(e.Message);
            return null;
        }

        NonceCounter += 1;

        var media = new Media()
        {
            Nonce = NonceCounter,
            Duration = response.DurationInSeconds,
            Title = response.Title,
            CanEmbed = response.CanEmbed,
            Thumbnail = response.Thumbnail,
            Verified = true,
            YouTubeId = request.RequestData,
            Requestor = request.Requestor,
            VotesFor = new List<IClient>() { request.Requestor },
        };

        return media;
    }
}
