global using System;
global using System.IO;
global using System.Linq;
global using System.Threading.Tasks;

global using Sandbox;
global using Sandbox.UI;
global using Sandbox.UI.Construct;

global using Cinema;
global using Cinema.UI;

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
        if ( Game.IsServer )
        {

        }

        if ( Game.IsClient )
        {
            _ = new Hud();
        }
    }

    [Event.Hotload]
    protected void ReloadGame()
    {
        if ( Game.IsServer )
        {

        }

        if ( Game.IsClient )
        {
            _ = new Hud();
        }
    }

    /// <summary>
    /// A client has joined the server. Make them a pawn to play with
    /// </summary>
    public override void ClientJoined(IClient client)
    {
        base.ClientJoined(client);

        // Create a pawn for this client to play with
        var pawn = new Player(client);
        client.Pawn = pawn;

    }

    //Why is this here? -ItsRifter
    /*public override void ClientSpawn()
    {
        Game.RootPanel = new UI.Hud();
    }*/
}
