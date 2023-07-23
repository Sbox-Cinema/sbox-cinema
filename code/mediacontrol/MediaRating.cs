using CinemaTeam.Plugins.Media;
using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cinema;

/// <summary>
/// Manages likes and dislikes for the currently playing media in a CinemaZone.
/// </summary>
public partial class MediaRating : EntityComponent<CinemaZone>, ISingletonComponent
{
    [ConVar.Replicated("media.rating.moneyreward")]
    public int MoneyPerLikePerMinute { get; set; } = 5;

    [Net]
    private IDictionary<IClient, bool> CurrentRatings { get; set; }
    private MediaController Controller => Entity.MediaController;

    /// <summary>
    /// Clears <c>CurrentRatings</c> and grants ratings-based rewards to players.
    /// </summary>
    public void FinalizeRatings(MediaRequest media, float watchTime)
    {
        if (media == null)
        {
            Log.Info("Cannot finalize ratings: no media was playing.");
            return;
        }
        var requestor = media.Requestor;
        var netRating = GetNetRating();
        // TODO: Log this to the audit log along with a list of all ratings.
        Log.Info($"{Entity.Name} video requested by {requestor} has net {netRating} likes.");
        switch (netRating)
        {
            case 0:
                // Video was mid, return without penalty.
                return;
            case < 0:
                // TODO: Implement a pentalty for posting cringe.
                return;
            default:
                // Video was good, award the requestor.
                GrantReward(requestor, netRating, (int)watchTime);
                return;
        }
    }

    public int GetNetRating()
    {
        var totalLikes = CurrentRatings.Where(r => r.Value).Count();
        var totalDislikes = CurrentRatings.Where(r => !r.Value).Count();
        return totalLikes - totalDislikes;
    }

    protected void GrantReward(IClient requestor, int netRating, int timePlayed)
    {
        if (requestor.Pawn is not Player player)
            return;

        var minutesPlayed = (int)Math.Max(1, MathF.Floor(timePlayed / 60));
        var moneyEarned = netRating * minutesPlayed * MoneyPerLikePerMinute;
        player.AddMoney(moneyEarned);
        Log.Info($"Awarded {requestor.Name} ${moneyEarned} for playing {Controller.CurrentMedia.GenericInfo.Title} and getting {netRating} likes.");
    }

    public bool HasRated(IClient client, bool isLike)
    {
        if (!CurrentRatings.ContainsKey(client))
            return false;
        return CurrentRatings[client] == isLike;
    }

    public bool CanAddRating(IClient client, bool isLike)
    {
        // The requestor of media cannot rate their own media.
        if (Controller.CurrentMedia.Requestor == client)
            return false;

        // A client may not rate media if they are not in the same zone.
        if (ClientHelper.GetNearestZone(client) != Entity)
            return false;

        // Any non-requestor who has not yet rated media may rate it.
        if (!CurrentRatings.ContainsKey(client))
            return true;

        // A client may not apply the same rating twice.
        if (CurrentRatings[client] == isLike)
            return false;

        return true;
    }

    [ConCmd.Server]
    public static void AddRating(int zoneId, int clientId, bool isLike)
    {
        var zone = Sandbox.Entity.FindByIndex(zoneId) as CinemaZone;
        var rating = zone.MediaRating;
        var client = ClientHelper.FindById(clientId);
        rating.AddRating(client, isLike);
    }

    public void AddRating(IClient client, bool isLike)
    {
        if (CurrentRatings.ContainsKey(client) && CurrentRatings[client] == isLike)
            return;

        if (Game.IsClient)
        {
            AddRating(Entity.NetworkIdent, client.NetworkIdent, isLike);
            return;
        }

        // Add the rating.
    }

    [ConCmd.Server]
    public static void RemoveRating(int zoneId, int clientId)
    {
        var zone = Sandbox.Entity.FindByIndex(zoneId) as CinemaZone;
        var rating = zone.MediaRating;
        var client = ClientHelper.FindById(clientId);
        rating.RemoveRating(client);
    }

    public void RemoveRating(IClient client)
    {
        if (!CurrentRatings.ContainsKey(client))
            return;

        if (Game.IsClient)
        {
            RemoveRating(Entity.NetworkIdent, client.NetworkIdent);
            return;
        }

        CurrentRatings.Remove(client);
    }
}
