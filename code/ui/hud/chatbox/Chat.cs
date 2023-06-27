using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using Cinema;
using Sandbox.UI;

namespace Cinema.UI;

public partial class Chat : Panel, IMenuScreen
{
    public static Chat Instance { get; set; }
    
    public bool IsOpen { get; protected set; }
    public string VisibleClass => IsOpen ? "visible" : "";

    public string Name => "Chat";
    
    public Chat()
    {
        Instance = this;
    }
    
    public Panel Canvas { get; protected set; }
    public TextEntry Input { get; protected set; }

    Queue<ChatRow> Rows = new();

    protected int MaxItems => 100;
    protected float MessageLifetime => 10f;
    

        [ClientRpc]
	public static void AddChatEntryConsole( string name, string message, string playerId = "0", bool isInfo = false )
	{
        Instance?.AddEntry( name, message, long.Parse( playerId ), isInfo );

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

	[ConCmd.Client( "chat.addinfo", CanBeCalledFromServer = true )]
	public static void AddInformation( string message )
	{
        Instance?.AddEntry( null, message );
	}
    

	[ConCmd.Server( "chat.say" )]
	public static void Say( string message )
	{
        if ( message.Contains( '\n' ) || message.Contains( '\r' ) )
			return;
        
		Log.Info( $"{ConsoleSystem.Caller}: {message}" );

        AddChatEntry( To.Everyone, ConsoleSystem.Caller.Name, message, ConsoleSystem.Caller.SteamId, false );
	}

    protected override void OnAfterTreeRender( bool firstTime )
	{
		base.OnAfterTreeRender( firstTime );

		Canvas.PreferScrollToBottom = true;
		Input.AcceptsFocus = true;
		Input.AllowEmojiReplace = true;
    }

	public override void Tick()
	{
        Input.Placeholder = string.IsNullOrEmpty( Input.Text ) ? "Enter your message..." : string.Empty;
		
		if (Game.LocalPawn is Player ply)
        {
            ply.CurrentlyTyping = HasClass("open") ? Input.Text : "";
        } 
	}

	public bool Open()
    {
        AddClass( "open" );
		Input.Focus();
		Canvas.TryScrollToBottom();
        
        IsOpen = true;

        return true;
    }

	public void Close()
	{
        RemoveClass("open");
		Input.Blur();

        IsOpen = false;
    }

	private void Submit()
    {
        var msg = Input.Text.Trim();
		Input.Text = "";

		if ( string.IsNullOrWhiteSpace( msg ) )
			return;

		Say( msg );
        
        IsOpen = false;
	}

	public void AddEntry( string name, string message, long playerId = 0, bool isInfo = false, bool isLastWords = false )
	{
		var e = Canvas.AddChild<ChatRow>();

		var player = Game.LocalPawn;
		if ( !player.IsValid() ) return;

		if ( playerId > 0 )
			e.PlayerId = playerId;

		e.Message = message;
		e.Name = $"{name}";
		

		e.SetClass( "noname", string.IsNullOrEmpty( name ) );
		e.SetClass( "info", isInfo );
		e.BindClass( "stale", () => e.Lifetime > MessageLifetime );

		var cl = Game.Clients.ToList().FirstOrDefault( x => x.SteamId == playerId );
		if ( cl.IsValid() )
			e.SetClass( "friend", cl.IsFriend || Game.SteamId == playerId );

		Canvas.TryScrollToBottom();

		Rows.Enqueue( e );

		// Kill an item if we need to
		if ( Rows.Count > MaxItems )
			Rows.Dequeue().Delete();
	}
}
