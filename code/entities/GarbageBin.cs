using System;
using Cinema.Jobs;
using Editor;
using Sandbox;

namespace Cinema;

[Library("ent_garbagebin"), HammerEntity]
[Model(Archetypes = ModelArchetype.animated_model, Model = "models/sbox_props/metal_wheely_bin/metal_wheely_bin.vmdl")]
[Title("Garbage Bin"), Category("Gameplay"), Icon("delete")]
public partial class GarbageBin : AnimatedEntity, ICinemaUse
{
    public static float UseDuration => 1.0f;

    public new string Name => "Garbage Bin";

    public bool TimedUse => true;

    public string UseText => "Empty Garbage Bag";

    public string CannotUseText => Garbage.GetPlayerGarbageBag(LocalPlayer).Contents.Count == 0 ? "Not carrying any garbage" : "Cannot use";

    public bool ShowCannotUsePrompt => LocalPlayer.Job.HasAbility(Jobs.JobAbilities.PickupGarbage);

    [Net]
    public Player BeingUsedBy { get; private set; }

    [Net]
    public TimeUntil TimeUntilFinishedUsing { get; private set; }

    public bool IsBeingUsed => BeingUsedBy is not null;

    public float TimedUsePercentage => IsBeingUsed ? Math.Min(TimeUntilFinishedUsing.Passed / UseDuration, 1) : 0;

    private static Player LocalPlayer => Game.LocalPawn as Player;

    public override void Spawn()
    {
        base.Spawn();
        Model = Cloud.Model("facepunch.metal_wheely_bin");
        SetupPhysicsFromModel(PhysicsMotionType.Static, false);
    }

    public bool IsUsable(Entity user)
    {
        if (user is not Player player) return false;
        if (IsBeingUsed) return BeingUsedBy == player;
        if (!player.Job.HasAbility(JobAbilities.PickupGarbage)) return false;
        if (Garbage.GetPlayerGarbageBag(player).Contents.Count == 0) return false;

        return true;
    }

    public bool OnStopUse(Entity user)
    {
        if (user is not Player player) return true;
        if (BeingUsedBy != player) return true;

        BeingUsedBy = null;

        return true;
    }

    public bool OnUse(Entity user)
    {
        if (user is not Player player) return false;

        if (!IsBeingUsed)
        {
            StartUsing(player);
        }

        if (TimeUntilFinishedUsing < 0)
        {
            FinishUsing();
            return false;
        }

        return true;
    }

    private void StartUsing(Player player)
    {
        BeingUsedBy = player;
        TimeUntilFinishedUsing = UseDuration;
    }

    private void FinishUsing()
    {
        var player = BeingUsedBy;
        BeingUsedBy = null;

        var garbageBag = Garbage.GetPlayerGarbageBag(player);
        if (!garbageBag.IsValid()) return;

        garbageBag.RemoveAll();
    }
}
