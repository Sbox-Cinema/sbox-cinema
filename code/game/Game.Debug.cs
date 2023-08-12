using System;
using System.Linq;
using Conna.Inventory;
using Sandbox;

namespace Cinema;

public partial class CinemaGame
{
    public static bool ValidateUser(long steamID) => true;

    private static IClient GetPlayer(string query)
        => Game.Clients.FirstOrDefault(
                cl => cl.Name.ToLower().Contains(query.ToLower())
            );

    [ClientRpc]
    public static void CleanupClientEntities()
    {
        Decal.Clear(true, true);
        foreach (Entity ent in Entity.All)
        {
            if (ent.IsClientOnly)
                ent.Delete();
        }
    }

    private static bool CanDeleteEntity(Entity ent)
    {
        if (ent is Player) return false;
        if (ent is WorldEntity) return false;

        return true;
    }

    [ConCmd.Server("game.reset")]
    public static void ResetGame()
    {
        if (!ValidateUser(ConsoleSystem.Caller.SteamId)) return;

        // Tell our game that all clients have just left.
        foreach (IClient cl in Game.Clients)
        {
            CinemaGame.Current.ClientDisconnect(cl, NetworkDisconnectionReason.SERVER_SHUTDOWN);
        }

        // Cleanup on clients
        CleanupClientEntities(To.Everyone);

        // Delete everything except the clients and the world 
        foreach (Entity ent in Entity.All)
        {
            if (ent is not IClient &&
                ent is not WorldEntity)
                ent.Delete();
        }


        // Reset the map, this will respawn all map entities
        Game.ResetMap(Entity.All.Where(x => x is Player).ToArray());

        // Create a brand new game
        CinemaGame.Current = new CinemaGame();

        // Fake a post level load after respawning entities, just incase something uses it
        CinemaGame.Current.PostLevelLoaded();

        // Tell our new game that all clients have just joined to set them all back up.
        foreach (IClient cl in Game.Clients)
        {
            CinemaGame.Current.ClientJoined(cl);
        }
    }

    [ConCmd.Server("ent.delete")]
    public static void DeleteEntity()
    {
        if (!ValidateUser(ConsoleSystem.Caller.SteamId)) return;

        if (ConsoleSystem.Caller.Pawn is not Player player) return;

        var tr = Trace.Ray(player.EyePosition, player.EyePosition + player.EyeRotation.Forward * 999)
            .Ignore(player)
            .Run();

        if (tr.Entity.IsValid && CanDeleteEntity(tr.Entity))
            tr.Entity.Delete();
    }

    [ConCmd.Server("ent.create")]
    public static void SpawnEntity(string entName)
    {
        if (!ValidateUser(ConsoleSystem.Caller.SteamId)) return;

        Log.Info("creating " + entName);

        var owner = ConsoleSystem.Caller.Pawn as Player;

        if (owner == null)
        {
            Log.Info("Failed to create " + entName);
            return;
        }

        var entityType = TypeLibrary.GetType<Entity>(entName)?.TargetType;
        if (entityType == null)
        {
            Log.Info("Failed to create " + entName);
            return;
        }

        var tr = Trace.Ray(owner.AimRay, 500)
            .UseHitboxes()
            .Ignore(owner)
            .Size(2)
            .Run();
        // might spawn as an entity prefab. Weird.
        var ent = TypeLibrary.Create<Entity>(entityType);

        ent.Position = tr.EndPosition;
        ent.Spawn();
        // Make the spawned entity face the player who spawned it.
        //ent.Rotation = Rotation.From(new Angles(15, (-owner.AimRay.Forward).EulerAngles.yaw, 15));
    }

    [ConCmd.Server("item.give")]
    public static void SpawnWeaponCMD(string itemId)
    {
        if (!ValidateUser(ConsoleSystem.Caller.SteamId)) return;

        if (ConsoleSystem.Caller.Pawn is not Player player) return;

        var item = InventorySystem.CreateItem(itemId);
        if (item == null) return;

        player.Inventory.Give(item);
    }

    // creates a world entity for usable items.
    [ConCmd.Server("item.create")]
    public static void CreateItemCMD(string itemId)
    {
        if (!ValidateUser(ConsoleSystem.Caller.SteamId)) return;

        if (ConsoleSystem.Caller.Pawn is not Player player) return;

        var item = InventorySystem.CreateItem(itemId);
        if (item == null) return;

        var entity = new ItemEntity();
        entity.SetItem(item);
        entity.Position = player.EyePosition + player.EyeRotation.Forward * 100;

    }


    [ConCmd.Server("player.bring")]
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
            .StaticOnly()
            .Run();

        client.Pawn.Position = tr.EndPosition;
        (client.Pawn as Player).ResetInterpolation();
    }

    [ConCmd.Server("money.give")]
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

    [ConCmd.Server("money.take")]
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

    [ConCmd.Server("player.job.set")]
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

    [ConCmd.Client("player.job.debug.client")]
    public static void DebugJobClient()
    {
        if (ConsoleSystem.Caller.Pawn is not Player player) return;
        Log.Info($"Job: {player.Job.Name}");
        // Doing .ToString() makes it so sandbox doesn't add a selectable underline
        Log.Info($"Abilities: {player.Job.Abilities.ToString()}");
        Log.Info($"Responsibilities: {player.Job.Responsibilities.ToString()}");
    }

    [ConCmd.Client("hud.enable")]
    public static void EnableHud(bool enable)
    {
        if (Game.RootPanel == null)
        {
            return;
        }
        Game.RootPanel.Style.Display = enable
            ? Sandbox.UI.DisplayMode.Flex
            : Sandbox.UI.DisplayMode.None;
    }

    [ConCmd.Server("player.list")]
    public static void ListPlayers()
    {
        if (!ValidateUser(ConsoleSystem.Caller.SteamId)) return;

        foreach (var client in Game.Clients)
        {
            var localPawn = client.Pawn as Player;
            string status;
            if (localPawn == null)
                status = "In Transit";
            else if (localPawn.LifeState == LifeState.Dead)
                status = "Dead";
            else if (string.IsNullOrEmpty(localPawn.ActiveMenuName))
                status = "Active";
            else
                status = $"In Menu ({localPawn.ActiveMenuName})";
            Log.Info($"{client} - {status}");
        }
    }

    public static void ListItems(Player player)
    {
        foreach (var item in player.Inventory.ItemList.WithIndex().Where(x => x.item.IsValid()))
        {
            Log.Info($"{item.index}: {item.item.UniqueId}");
        }
    }


    [ConCmd.Client("item.list.cl")]
    public static void ListItemsClient()
    {
        if (ConsoleSystem.Caller.Pawn is not Player player) return;

        ListItems(player);
    }

    [ConCmd.Server("item.list.sv")]
    public static void ListItemsServer()
    {
        if (ConsoleSystem.Caller.Pawn is not Player player) return;

        ListItems(player);
    }

    [ConCmd.Server("player.strip.weapons")]
    public static void StripWeapons(string playerName)
    {
        if (!ValidateUser(ConsoleSystem.Caller.SteamId)) return;

        var client = GetPlayer(playerName);

        if (client?.Pawn is not Player player)
            return;

        player
            .Weapons
            .ToList()
            .ForEach(w => w.RemoveFromHolder());
    }
}
