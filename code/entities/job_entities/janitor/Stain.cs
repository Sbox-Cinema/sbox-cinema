using Editor;
using Sandbox;
using Sandbox.Component;
using System;
using System.Linq;

namespace Cinema;

public partial class Stain : Entity
{
    Particles stainParticle;
    StainTriggerBox stainBox;

    public override void Spawn()
    {
        base.Spawn();
    }

    public void MakeStain()
    {

    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (Game.IsServer)
        {
            stainParticle.Destroy();
        }
    }

    public void CleanStain(Player player)
    {
        //TODO: Pay the janitor

        Delete();
    }

    /*public bool OnUse(Entity user)
    {
        if (!IsUsable(user)) return false;

        CleanStain(user as Player);
        return false;
    }

    public bool IsUsable(Entity user)
    {
        if (user is Player player && player.Job.Abilities == Jobs.JobAbilities.PickupGarbage)
            return true;

        return false;
    }*/

    //TEMP: Will remove at some point
    [ConCmd.Server("cinema.stain.test")]
    public static void TestStain()
    {
        var player = ConsoleSystem.Caller.Pawn as Player;
        if (player == null) return;

        var forward = player.AimRay.Position + player.AimRay.Forward * 999;

        var tr = Trace.Ray(player.EyePosition, forward)
            .WorldOnly()
            .Run();

        /*Stain stain = new Stain();
        stain.Position = tr.EndPosition;
        stain.MakeStain();*/
    }
}

public class StainTriggerBox : BaseTrigger, IUse
{
    public override void Spawn()
    {
        base.Spawn();
    }

    public bool OnUse(Entity user)
    {
        if (!IsUsable(user)) return false;

        var stain = Parent as Stain;
        if (stain == null) return false;

        stain.CleanStain(user as Player);

        return false;
    }

    public bool IsUsable(Entity user)
    {
        if (user is Player player && player.Job.Abilities == Jobs.JobAbilities.PickupGarbage)
            return true;

        return false;
    }
}
