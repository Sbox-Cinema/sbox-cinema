using System;
using System.Linq;
using Sandbox;

namespace Cinema;

public partial class CinemaGame
{
    public static long[] DevIDs => new long[]
    {
        //Remscar
        76561198001764190,
		//ItsRifter
		76561197972285500,
		//E1M1 (Josh)
		76561197985922183,
		//Baik
		76561197991940798,
        //Walker / eXodos
        76561197973408108,
    };

    public static bool ValidateUser(long steamID)
    {
        if (DevIDs.Contains(steamID)) return true;

        return false;
    }

    [ConCmd.Server("cinema.entity.delete")]
    public static void CommandExample()
    {
        if (!ValidateUser(ConsoleSystem.Caller.SteamId)) return;

        if (ConsoleSystem.Caller.Pawn is not Player player) return;

        var tr = Trace.Ray(player.EyePosition, player.EyePosition + player.EyeRotation.Forward * 999)
            .Ignore(player)
            .Run();

        if (tr.Entity.IsValid && CanDeleteEntity(tr.Entity))
            tr.Entity.Delete();
    }

    private static bool CanDeleteEntity(Entity ent)
    {
        if (ent is Player) return false;
        if (ent is WorldEntity) return false;

        return true;
    }

    [ConCmd.Server("cinema.weapon.create")]
    public static void SpawnWeaponCMD(string wepName, bool inInv = false)
    {
        if (!ValidateUser(ConsoleSystem.Caller.SteamId)) return;

        if (ConsoleSystem.Caller.Pawn is not Player player) return;

        var wep = CreateByName<WeaponBase>(wepName);
        if (wep == null) return;

        if (inInv)
        {
            player.Inventory.AddWeapon(wep, true);
        }
        else
        {
            var tr = Trace.Ray(player.EyePosition, player.EyePosition + player.EyeRotation.Forward * 999)
                .Ignore(player)
                .WorldOnly()
                .Run();

            wep.Position = tr.EndPosition;
        }
    }

    [ConCmd.Server("cinema.player.bring")]
    public static void BringPlayerCMD(string playerName)
    {
        if (!ValidateUser(ConsoleSystem.Caller.SteamId)) return;

        if (ConsoleSystem.Caller.Pawn is not Player player) return;

        var clients = Game.Clients.Where(x => x.Name.ToLower().Contains(playerName));

        if (clients.Count() is <= 0 or > 1) return;

        var client = clients.FirstOrDefault();

        if (client == ConsoleSystem.Caller.Client) return;

        var tr = Trace.Ray(player.EyePosition, player.EyePosition + player.EyeRotation.Forward * 999)
            .Ignore(player)
            .WorldOnly()
            .Run();

        client.Pawn.Position = tr.EndPosition;
        (client.Pawn as Player).ResetInterpolation();
    }

    [ConCmd.Server("cinema.player.money.give")]
    public static void GivePlayerCashCMD(int amt, string playerName = "")
    {
        if (!ValidateUser(ConsoleSystem.Caller.SteamId)) return;

        if (amt <= 0) return;

        if (ConsoleSystem.Caller.Pawn is not Player player) return;

        if (string.IsNullOrEmpty(playerName))
            player.AddMoney(amt);
        else
        {
            var clients = Game.Clients.Where(x => x.Name.ToLower().Contains(playerName));

            if (clients.Count() is <= 0 or > 1) return;

            var client = clients.FirstOrDefault();

            if (client == ConsoleSystem.Caller.Client || client.Pawn is not Player given) return;

            given.AddMoney(amt);
        }
    }

    [ConCmd.Server("cinema.player.money.take")]
    public static void TakePlayerCashCMD(int amt, string playerName = "")
    {
        if (!ValidateUser(ConsoleSystem.Caller.SteamId)) return;

        if (amt <= 0) return;

        if (ConsoleSystem.Caller.Pawn is not Player player) return;

        if (string.IsNullOrEmpty(playerName))
            player.TakeMoney(amt);
        else
        {
            var clients = Game.Clients.Where(x => x.Name.ToLower().Contains(playerName));

            if (clients.Count() is <= 0 or > 1) return;

            var client = clients.FirstOrDefault();

            if (client == ConsoleSystem.Caller.Client || client.Pawn is not Player given) return;

            given.TakeMoney(amt);
        }
    }

    [ConCmd.Server("cinema.npc.create")]
    public static void SpawnNpcCMD(string npcName)
    {
        if (!ValidateUser(ConsoleSystem.Caller.SteamId)) return;

        var player = ConsoleSystem.Caller.Pawn as Player;
        if (player == null) return;

        var npc = CreateByName<NpcBase>(npcName);
        if (npc == null) return;

        var tr = Trace.Ray(player.EyePosition, player.EyePosition + player.EyeRotation.Forward * 999)
            .Ignore(player)
            .WorldOnly()
            .Run();

        npc.Position = tr.EndPosition;
    }

    [ConCmd.Server("cinema.player.job.set")]
    public static void SetJob(string jobName, string playerName = "")
    {
        if (!ValidateUser(ConsoleSystem.Caller.SteamId)) return;

        if (ConsoleSystem.Caller.Pawn is not Player player) return;

        var target = player;

        if (!string.IsNullOrEmpty(playerName))
        {
            var clients = Game.Clients.Where(x => x.Name.ToLower().Contains(playerName));

            if (clients.Count() is <= 0 or > 1) return;

            var client = clients.FirstOrDefault();

            if (client == ConsoleSystem.Caller.Client || client.Pawn is not Player given) return;

            target = given;
        }

        var details = Jobs.JobDetails.All.FirstOrDefault(x => x.Name.ToLower().Contains(jobName.ToLower()));

        if (details == null) return;

        target.SetJob(details);
    }

    [ConCmd.Client("cinema.player.job.debug.client")]
    public static void DebugJobClient()
    {
        if (ConsoleSystem.Caller.Pawn is not Player player) return;
        Log.Info($"Job: {player.Job.JobDetails.Name}");
        Log.Info($"Abilities: {player.Job.JobDetails.Abilities.ToNiceBinary()}");
    }
}
