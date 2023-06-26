using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using Cinema;
using Sandbox.UI;

namespace Cinema.UI;

public partial class Chat : Panel
{
	[ClientRpc]
	public static void AddChatEntryConsole( string name, string message, string playerId = "0", bool isInfo = false )
	{
        Current?.AddEntry( name, message, long.Parse( playerId ), isInfo );

		// Only log clientside if we're not the listen server host
		if ( !Game.IsListenServer )
		{
			Log.Info( $"{name}: {message}" );
		}
	}

	public static void AddChatEntry( To target, string name, string message, long playerId = 0, bool isInfo = false )
	{
		AddChatEntryConsole( target, name, message, playerId.ToString(), isInfo );
	}

	[ConCmd.Client( "sandbox_addinfo", CanBeCalledFromServer = true )]
	public static void AddInformation( string message )
	{
		Current?.AddEntry( null, message );
	}
    

	[ConCmd.Server( "chat_say" )]
	public static void Say( string message )
	{
        if ( message.Contains( '\n' ) || message.Contains( '\r' ) )
			return;
        
		Log.Info( $"{ConsoleSystem.Caller}: {message}" );

        AddChatEntry( To.Everyone, ConsoleSystem.Caller.Name, message, ConsoleSystem.Caller.SteamId, false );
	}
}
