using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

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
            safePos = Vector3.Zero;
        }
        else
        {
            AttemptUnstuck();
        }
    }

    //Check if the player is stuck
    protected bool IsStuck()
    {
        var traceBox = Controller.TraceBBox(Controller.Position, Controller.Position);

        return traceBox.StartedSolid;
    }

    protected bool AttemptUnstuck()
    {
        //Only the server should unstuck the player
        if (Game.IsClient) return true;

        int triesPerTick = 20;

        for (int i = 0; i < triesPerTick; i++)
        {
            //var pos = Controller.Position + Vector3.Random.Normal * (UnstuckAttempts / 2.0f);
            var pos = Controller.Position + (Vector3.Random * 45 * UnstuckAttempts);
            DebugOverlay.Line(Controller.Position, pos, 5.0f);

            var check = Controller.TraceBBox(pos, Controller.Position);
            if (ValidatePosition(check))
            {
                Controller.Position = safePos;
                return false;
            }

            check = Controller.TraceBBox(pos, pos + Vector3.Down * 64.0f);
            if (ValidatePosition(check))
            {
                Controller.Position = safePos;
                return false;
            }
        }

        UnstuckAttempts++;

        return true;
    }

    Vector3 safePos;

    protected bool ValidatePosition(TraceResult result)
    {
        if (result.StartedSolid) return false;
        if (result.Normal.z != 1) return false;

        safePos = result.HitPosition;
        return true;
    }
}
