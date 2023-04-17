using Sandbox;
using Sandbox.UI;
using System;

namespace Cinema.UI;

public partial class ArcadeTest : Panel
{
    public MyArcadeGame GameController { get; set; }
    public ArcadeTest(MyArcadeGame game)
    {
        GameController = game;
    }

    public bool Started => GameController.GameStarted;

    protected override int BuildHash()
    {
        return HashCode.Combine(Started);
    }

}
