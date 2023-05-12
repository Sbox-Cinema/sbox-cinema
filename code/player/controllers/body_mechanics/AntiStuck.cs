using Sandbox;
namespace Cinema;
public partial class AntiStuckMechanic : PlayerBodyControllerMechanic
{
    internal int UnstuckAttempts = 0;

    protected override void Simulate()
    {
        if( !IsStuck() )
        {
            //Reset unstuck attempts if they aren't actually stuck
            UnstuckAttempts = 0;
            return;
        }

        AttemptUnstuck();
    }

    //Check if the player is stuck
    protected bool IsStuck()
    {
        var traceBox = Controller.TraceBBox(Controller.Position, Controller.Position);

        return traceBox.StartedSolid;
    }

    protected void AttemptUnstuck()
    {
        //Only the server should unstuck the player
        if (Game.IsClient) return;

        int triesPerTick = 20;

        for (int i = 0; i < triesPerTick; i++)
        {
            var pos = Controller.Position + (Vector3.Random * 45 * UnstuckAttempts);

            var check = Controller.TraceBBox(pos, Controller.Position);
            if (ValidateTrace(check))
            {
                Controller.Position = check.EndPosition;
                return;
            }

            check = Controller.TraceBBox(pos, pos + Vector3.Down * 64.0f);
            if (ValidateTrace(check))
            {
                Controller.Position = check.EndPosition;
                return;
            }
        }

        UnstuckAttempts++;
    }

    protected bool ValidateTrace(TraceResult result)
    {
        if (result.StartedSolid) return false;
        if (result.Normal.z != 1) return false;

        return true;
    }
}
