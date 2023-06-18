using Sandbox;
using Sandbox.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cinema.UI;

public partial class Scoreboard : Panel, IMenuScreen
{
    public static Scoreboard Instance { get; set; }
    public string Name => Game.Server.ServerTitle;
    public bool IsOpen { get; protected set; }
    public string VisibleClass => IsOpen ? "visible" : "";
    public IReadOnlyCollection<IClient> Clients => Game.Clients;

    public Scoreboard()
    {
        Instance = this;

        
    }
    public bool Open()
    {
        IsOpen = true;
        
        return true;
    }
    public void Close()
    {
        IsOpen = false;
    }
    public override void Tick()
    {
        base.Tick();
    }
    protected override int BuildHash()
    {
        var queueHash = 11;

        return HashCode.Combine(IsOpen, queueHash, Clients);
    }
}
