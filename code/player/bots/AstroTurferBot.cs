using Sandbox;
using Sandbox.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cinema;

public class AstroTurferBot : CinemaBot
{
    private static Dictionary<CinemaZone, List<AstroTurferBot>> Astroturfers { get; set; } = new();
    public CinemaChair AssignedSeat { get; set; }

    public static IEnumerable<AstroTurferBot> GetBots(CinemaZone zone)
    {
        if (!Astroturfers.ContainsKey(zone))
        {
            return Enumerable.Empty<AstroTurferBot>();
        }
        return Astroturfers[zone];
    }

    [ConCmd.Admin("bot.astro.spawn")]
    public static void SpawnAstroTurfer()
    {
        if (ConsoleSystem.Caller?.Pawn is not Player ply)
            return;

        var capsule = Capsule.FromHeightAndRadius(64f, 32f);
        var tr = Trace
            .Capsule(capsule, ply.AimRay, 100f)
            .WithTag("chair")
            .Run();

        if (tr.Entity is not CinemaChair chair || chair.IsOccupied)
            return;

        var bot = new AstroTurferBot();
        bot.AssignedSeat = chair;
        foreach(var zone in Entity.All.OfType<CinemaZone>())
        {
            if (!zone.ProjectorEntity.IsValid())
                continue;

            if (zone.WorldSpaceBounds.Contains(chair.WorldSpaceBounds))
            {
                if (!Astroturfers.ContainsKey(zone))
                {
                    Astroturfers[zone] = new List<AstroTurferBot>();
                }
                Astroturfers[zone].Add(bot);
                break;
            }
        }
    }

    [ConCmd.Admin("bot.astro.queue")]
    public static async void QueueTestMedia(int zoneId)
    {
        var zone = Entity.FindByIndex<CinemaZone>(zoneId);

        if (!zone.IsValid())
            return;

        var randomBot = Astroturfers[zone].OrderBy(_ => Guid.NewGuid()).First();
        var youTubeProvider = VideoProviderManager
            .Instance
            .GetAll()
            .FirstOrDefault(p => p.ProviderName.ToLower().Contains("youtube"));

        if (youTubeProvider == null)
        {
            Log.Info("No YouTube media provider was found.");
            return;
        }

        var randomVideo = GoodVideos.OrderBy(_ => Guid.NewGuid()).First();
        var request = await youTubeProvider.CreateRequest(randomBot.Client, randomVideo);
        zone.MediaQueue.Push(request);
    }

    private static void RateCurrentMedia(CinemaZone zone, bool rating)
    {
        if (!zone.IsValid())
            return;

        var notYetRated = Astroturfers[zone]
            .Where(at => zone.MediaRating.CanAddRating(at.Client, rating))
            .OrderBy(_ => Guid.NewGuid())
            .FirstOrDefault();
        if (!notYetRated.IsValid())
        {
            Log.Info("No astroturfer in this zone has not yet rated the current media.");
            return;
        }
        zone.MediaRating.AddRating(notYetRated.Client, rating);
    }

    [ConCmd.Admin("bot.astro.like")]
    public static void LikeCurrentMedia()
    {
        var zone = GetConsoleCallerZone();

        if (!zone.IsValid())
            return;

        RateCurrentMedia(zone, true);
    }

    [ConCmd.Admin("bot.astro.dislike")]
    public static void DislikeCurrentMedia()
    {
        var zone = GetConsoleCallerZone();

        if (!zone.IsValid())
            return;

        RateCurrentMedia(zone, false);
    }

    /// <summary>
    /// Returns the bot who has rated the current media with the specfieid rating,
    /// or null if no such bot is found.
    /// </summary>
    private static AstroTurferBot GetBotHavingRated(CinemaZone zone, bool rating)
    {
        if (!zone.IsValid())
            return null;

        var botHavingRated = Astroturfers[zone]
            .Where(at => zone.MediaRating.HasRated(at.Client, rating))
            .OrderBy(_ => Guid.NewGuid())
            .FirstOrDefault();

        return botHavingRated;
    }

    [ConCmd.Admin("bot.astro.unlike")]
    public static void UnLikeCurrentMedia()
    {
        var zone = GetConsoleCallerZone();

        if (!zone.IsValid())
            return;

        var botHavingLiked = GetBotHavingRated(zone, true);
        if (!botHavingLiked.IsValid())
        {
            Log.Info($"No astroturfer in this zone has liked the current media.");
            return;
        }

        zone.MediaRating.RemoveRating(botHavingLiked.Client);
    }

    [ConCmd.Admin("bot.astro.undislike")]
    public static void UnDislikeCurrentMedia()
    {
        var zone = GetConsoleCallerZone();

        if (!zone.IsValid())
            return;

        var botHavingLiked = GetBotHavingRated(zone, false);
        if (!botHavingLiked.IsValid())
        {
            Log.Info($"No astroturfer in this zone has disliked the current media.");
            return;
        }

        zone.MediaRating.RemoveRating(botHavingLiked.Client);
    }

    /// <summary>
    /// Retrieve the <c>CinemaZone</c> containing the player who called the console command.
    /// If no such zone is found or the zone contains no astroturfer bots, return <c>null</c>.
    /// </summary>
    private static CinemaZone GetConsoleCallerZone()
    {
        if (ConsoleSystem.Caller?.Pawn is not Player ply)
        {
            Log.Info("This command must be called as a player.");
            return null;
        }

        var zone = ply.GetCurrentTheaterZone();
        if (zone == null)
        {
            Log.Info("Not in a theater zone.");
            return null;
        }

        if (!Astroturfers.ContainsKey(zone) || !Astroturfers[zone].Any())
        {
            Log.Info("Cannot find astroturfer in current cinema zone.");
            return null;
        }

        return zone;
    }

    public override void OnRespawn()
    {
        base.OnRespawn();

        AssignedSeat.OnUse(Client.Pawn as Player);
    }

    private static List<string> GoodVideos = new()
    {
        "https://www.youtube.com/watch?v=QH2-TGUlwu4", // Nyan cat
        "https://www.youtube.com/watch?v=wKbU8B-QVZk", // I can't believe you've done this
        "https://www.youtube.com/watch?v=FtutLA63Cp8", // Bad Apple
        "https://www.youtube.com/watch?v=ygI-2F8ApUM", // BrodyQuest
        "https://www.youtube.com/watch?v=irU_2h60T50", // Earthbound, Mr. Carpainter fight
        "https://www.youtube.com/watch?v=0ee0syZi9E0", // Royco Cup-a-Soup
        "https://www.youtube.com/watch?v=aaLrCdIsTPs", // Earthbound, Titanic Ant fight (in 4:3 resolution)
        "https://www.youtube.com/shorts/iQWY6j4aGc8", // Skibidi Toilet 6 (in vertical resolution)
    };
}
