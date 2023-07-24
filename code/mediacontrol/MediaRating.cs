using CinemaTeam.Plugins.Media;
using Sandbox;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
                break;
            case < 0:
                // TODO: Implement a pentalty for posting cringe.
                break;
            default:
                // Video was good, award the requestor.
                GrantReward(requestor, netRating, (int)watchTime);
                break;
        }
        CurrentRatings.Clear();
    }

    /// <summary>
    /// Returns the net rating of the current media. This number is calculated by subtracting the 
    /// number of dislikes from the number of likes.
    /// </summary>
    public int GetNetRating()
    {
        var totalLikes = CurrentRatings.Where(r => r.Value).Count();
        var totalDislikes = CurrentRatings.Where(r => !r.Value).Count();
        return totalLikes - totalDislikes;
    }

    /// <summary>
    /// Give the specified client rewards appropriate for the rating and playtime of
    /// the media that has been played.
    /// </summary>
    protected void GrantReward(IClient requestor, int netRating, int timePlayed)
    {
        if (requestor.Pawn is not Player player)
            return;

        var minutesPlayed = (int)Math.Max(1, MathF.Floor(timePlayed / 60));
        var moneyEarned = netRating * minutesPlayed * MoneyPerLikePerMinute;
        player.AddMoney(moneyEarned);
        Log.Info($"Awarded {requestor.Name} ${moneyEarned} for playing {Controller.CurrentMedia.GenericInfo.Title} and getting {netRating} likes.");
    }

    /// <summary>
    /// Returns whether the specified client has rated the current media.
    /// </summary>
    public bool HasRated(IClient client)
    {
        return Controller.CurrentMedia != null && CurrentRatings.ContainsKey(client);
    }

    /// <summary>
    /// Returns whether the specified client has rated the current media with the specified rating.
    /// </summary>
    public bool HasRated(IClient client, bool isLike)
    {
        if (Controller.CurrentMedia == null || !CurrentRatings.ContainsKey(client))
            return false;
        return CurrentRatings[client] == isLike;
    }

    /// <summary>
    /// Returns whether the specified client may give the specified rating to the current media.
    /// </summary>
    public bool CanAddRating(IClient client, bool isLike)
    {
        if (Controller.CurrentMedia == null)
        {
            Log.Trace($"{client} - Cannot add rating: no media is playing.");
            return false;
        }

        if (Controller.CurrentMedia.Requestor == client)
        {
            Log.Trace($"{client} - Cannot add rating: requestor cannot rate their own media.");
            return false;
        }

        if (ClientHelper.GetNearestZone(client) != Entity)
        {
            Log.Trace($"{client} - Cannot add rating: client is not in the same zone as the media.");
            return false;
        }

        // Any non-requestor who has not yet rated media may rate it.
        if (!CurrentRatings.ContainsKey(client))
            return true;

        if (CurrentRatings[client] == isLike)
        {
            Log.Trace($"{client} - Cannot add rating: client has already applied this rating.");
            return false;
        }

        return true;
    }

    /// <summary>
    /// Returns whether the specified client may remove their rating from the current media.
    /// </summary>
    public bool CanRemoveRating(IClient client)
    {
        // No rating may be removed if no media is playing.
        if (Controller.CurrentMedia == null)
            return false;

        // If no rating already exists for this client, it cannot be removed.
        if (!HasRated(client))
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

    /// <summary>
    /// If possible, adds the specified rating to the current media on behalf of the specified client.
    /// </summary>
    public void AddRating(IClient client, bool isLike)
    {
        if (!CanAddRating(client, isLike))
            return;

        if (Game.IsClient)
        {
            AddRating(Entity.NetworkIdent, client.NetworkIdent, isLike);
            return;
        }

        // TODO: Log this to the audit log.
        Log.Info($"{Entity.Name} - Rating by {client}: {(isLike ? "like" : "dislike")}");
        CurrentRatings[client] = isLike;
        SpawnRatingEffect(client.Pawn as Entity, isLike);
    }

    [ClientRpc]
    public void SpawnRatingEffect(Entity target, bool isLike)
    {
        if (!target.IsValid())
            return;

        /* TODO: To allow these particles to be shipped with the Cinema game, we must either:
         * 1. Move the particles to the Cinema game project
         * 2. Create an asset like a model or GameResource that depends on the particles
         * 3. Upload the particles to asset party and mount them at the start of the game
         * 4. Wait for Facepunch to implement Cloud.Particle and use that here
         */
        var effectPath = isLike
            ? "particles/voting/upvote_01.vpcf"
            : "particles/voting/downvote_01.vpcf";
        var effect = Particles.Create(effectPath);
        effect.SetEntityAttachment(0, target, "eyes", Vector3.Zero.WithZ(16f));
    }

    [ConCmd.Server]
    public static void RemoveRating(int zoneId, int clientId)
    {
        var zone = Sandbox.Entity.FindByIndex(zoneId) as CinemaZone;
        var rating = zone.MediaRating;
        var client = ClientHelper.FindById(clientId);
        rating.RemoveRating(client);
    }

    /// <summary>
    /// If possible, removes the specified client's rating from the current media.
    /// </summary>
    public void RemoveRating(IClient client)
    {
        if (!CanRemoveRating(client))
            return;

        if (Game.IsClient)
        {
            RemoveRating(Entity.NetworkIdent, client.NetworkIdent);
            return;
        }

        var oldRating = CurrentRatings[client];
        // TODO: Log this to the audit log.
        Log.Info($"{Entity.Name} - Removing rating by {client}. Removed rating: {(oldRating ? "like" : "dislike")}");
        CurrentRatings.Remove(client);
    }
}
