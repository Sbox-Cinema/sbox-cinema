using Conna.Inventory;
using Cinema.UI;
using Sandbox;
using Cinema.player.bots;

//
// You don't need to put things in a namespace, but it doesn't hurt.
//
namespace Cinema;

/// <summary>
/// This is your game class. This is an entity that is created serverside when
/// the game starts, and is replicated to the client. 
/// 
/// You can use this to create things like HUDs and declare which player class
/// to use for spawned players.
/// </summary>
public partial class CinemaGame : GameManager
{
    public CinemaGame()
    {
        MountAssets();
    }

    public override void Spawn()
    {
        InventorySystem.Initialize();
        base.Spawn();
    }

    public override void ClientSpawn()
    {
        InventorySystem.Initialize();
        Game.RootPanel = new UI.Hud();
    }

    /// <summary>
    /// A client has joined the server. Make them a pawn to play with
    /// </summary>
    public override async void ClientJoined(IClient client)
    {
        base.ClientJoined(client);

        if (client.IsBot)
        {
            // Wait a frame for botId in ClientData to become available.
            await GameTask.DelaySeconds(Time.Delta);
        }

        // Create a pawn for this client to play with
        var pawn = new Player();
        pawn.MakePawnOf(client);
        pawn.Money = 500; // @TEMP: Give players money when they join
        pawn.Respawn();
        if (client.IsBot)
        {
            var bot = client.GetBot();
            if (bot.SpawnPosition.HasValue)
            {
                pawn.Position = bot.SpawnPosition.Value;
            }
        }
        
        Chat.AddChatEntry( To.Everyone, "Server", $"{client.Name} has joined the game", isInfo: true );
    }

    public override void ClientDisconnect(IClient client, NetworkDisconnectionReason reason)
    {
        base.ClientDisconnect(client, reason);
        
        Chat.AddChatEntry( To.Everyone, "Server", $"{client.Name} has left the game", isInfo: true );
    }
}
