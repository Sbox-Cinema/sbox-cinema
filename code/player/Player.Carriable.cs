using Sandbox;

namespace Cinema;

public partial class Player
{
    [Net, Predicted]
    public Carriable ActiveChild { get; set; }

    /// <summary>
    /// This isn't networked, but it's predicted. If it wasn't then when the prediction system
    /// re-ran the commands LastActiveChild would be the value set in a future tick, so ActiveEnd
    /// and ActiveStart would get called multiple times and out of order, causing all kinds of pain.
    /// </summary>
    [Predicted]
    Carriable LastActiveChild { get; set; }

    public virtual void SimulateActiveChild(IClient cl)
    {
        if (LastActiveChild != ActiveChild)
        {
            Log.Info("Active child changed");
            OnActiveChildChanged(LastActiveChild, ActiveChild);
            LastActiveChild = ActiveChild;
        }

        if (!LastActiveChild.IsValid())
            return;

        if (LastActiveChild.IsAuthority)
        {
            LastActiveChild.Simulate(cl);
        }
    }

    /// <summary>
    /// Called when the Active child is detected to have changed
    /// </summary>
    public virtual void OnActiveChildChanged(Carriable previous, Carriable next)
    {
        if (previous is Carriable previousBc)
        {
            previousBc?.ActiveEnd(this, previousBc.Owner != this);
        }

        if (next is Carriable nextBc)
        {
            Log.Info($"Carriable changed to: {nextBc?.Name}");
            nextBc?.ActiveStart(this);
        }
    }

    /// <summary>
    /// Sets the active child (carriable)
    /// If this is called on the client it tells the server.
    /// </summary>
    /// <param name="nextChild">The carriable to make active</param>
    public void ChangeActiveChild(Carriable nextChild)
    {
        // Note: I kinda hate this, maybe we use ClientInput to send active child
        ActiveChild = nextChild;
        if (Game.IsClient) ClientSetActiveChild(nextChild.NetworkIdent);
    }

    [ConCmd.Server]
    private static void ClientSetActiveChild(int nextChildIdent)
    {
        var nextChild = Entity.FindByIndex(nextChildIdent) as Carriable;
        if (!nextChild.IsValid()) return;
        (ConsoleSystem.Caller.Pawn as Player).ActiveChild = nextChild;
    }

}
