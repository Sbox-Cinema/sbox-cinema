﻿using Sandbox;
using Sandbox.Component;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema;

public partial class Trash : Prop, IUse
{
    Glow glow;

    public override void Spawn()
    {
        Tags.Add("solid, junk");

        SetupPhysicsFromModel(PhysicsMotionType.Static);
    }

    [GameEvent.Client.Frame]
    protected void ClientFrameS()
    {
        UpdateGlow();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (Game.IsServer)
        {
            var janitors = Game.Clients.Where(x => x.Pawn is Player pawn &&
            pawn.Job.JobDetails.Abilities == Jobs.JobAbilities.PickupGarbage);

            DestroyGlow(To.Multiple(janitors));
        }
    }

    [ClientRpc]
    void CreateGlow()
    {
        glow = Components.GetOrCreate<Glow>();
        glow.Color = Color.Cyan;
        glow.ObscuredColor = Color.Transparent;
    }

    void UpdateGlow()
    {
        var player = Game.LocalPawn as Player;

        if (glow == null)
        {
            if (player.Job.JobDetails.Abilities == Jobs.JobAbilities.PickupGarbage)
                CreateGlow();
            else return;
        } 
        else
        {
            if (player.Job.JobDetails.Abilities != Jobs.JobAbilities.PickupGarbage)
                DestroyGlow();
        }
    }

    [ClientRpc]
    void DestroyGlow()
    {
        Components.RemoveAny<Glow>();
        glow = null;
    }

    public void SetUp(Vector3 pos)
    {
        var tr = Trace.Ray(pos, pos + Vector3.Down * 32.0f).WorldOnly().Run();

        Position = tr.EndPosition + Vector3.Up * 3;
        Rotation = Rotation.FromYaw(Game.Random.Float(-90f, 180f)) + Rotation.FromPitch(90.0f);

        var janitors = Game.Clients.Where(x => x.Pawn is Player pawn && 
        pawn.Job.JobDetails.Abilities == Jobs.JobAbilities.PickupGarbage);

        CreateGlow(To.Multiple(janitors));

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
