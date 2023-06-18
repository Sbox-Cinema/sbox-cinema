using Sandbox;
using Sandbox.UI;
using System;

namespace Cinema.UI;

public partial class Scoreboard : Panel, IMenuScreen
{
    public static Scoreboard Instance { get; set; }
    public string Name => "Scoreboard";
    public bool IsOpen { get; protected set; }
    public string VisibleClass => IsOpen ? "visible" : "";

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

        return HashCode.Combine(IsOpen, queueHash);
    }
}
