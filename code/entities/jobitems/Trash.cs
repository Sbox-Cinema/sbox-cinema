using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema;

public class Trash : Prop, IUse
{
    public override void Spawn()
    {
        Tags.Add("solid, junk");

        SetupPhysicsFromModel(PhysicsMotionType.Static);
        Transmit = TransmitType.Always;
    }

    public void SetUp(Vector3 pos)
    {
        var tr = Trace.Ray(pos, pos + Vector3.Down * 32.0f).WorldOnly().Run();

        Position = tr.EndPosition + Vector3.Up * 3;
        Rotation = Rotation.FromYaw(Game.Random.Float(-90f, 180f)) + Rotation.FromPitch(90.0f);

        //Can we break the object into its gibs but not delete it? -ItsRifter
        //Breakables.Break(this);
    }

    private async void DisableGibPhysics()
    {
        await Task.DelaySeconds(1.0f);
    }

    public void Pickup(Player user)
    {
        Delete();
    }

    public bool OnUse(Entity user)
    {
        if (!IsUsable(user)) return false;

        Pickup(user as Player);
        return false;
    }

    public bool IsUsable(Entity user)
    {
        if (user is Player player && player.Job.Abilities == Jobs.JobAbilities.PickupGarbage)
            return true;

        return false;
    }
}
