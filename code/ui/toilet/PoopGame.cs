using System;
using Sandbox.UI;

namespace Cinema.UI;

public partial class PoopGame : Panel, IMenuScreen
{
    public static PoopGame Instance { get; set; }
    public bool IsOpen { get; private set; }

    protected string VisibleClass => IsOpen ? "visible" : "";

    public string Name => "Poop Game";

    public PoopGame()
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

    protected override int BuildHash()
    {
        return HashCode.Combine(IsOpen);
    }
}
