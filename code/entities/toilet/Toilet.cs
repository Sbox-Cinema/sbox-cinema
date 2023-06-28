using System;
using Cinema.Jobs;
using Editor;
using Sandbox;

namespace Cinema;

[Library("ent_toilet"), HammerEntity]
[Model(Archetypes = ModelArchetype.animated_model, Model = "models/cinema_toilet.vmdl")]
[Title("Toilet"), Category("Gameplay"), Icon("delete"), Description("Sit down, take a load off.")]
public partial class Toilet : AnimatedEntity, ICinemaUse
{
    public new string Name => "Toilet";

    public string UseText => "Use Toilet";

    public string CannotUseText => "Don't need to use the toilet right now";

    public bool ShowCannotUsePrompt => BeingUsedBy != LocalPlayer;

    public bool ToggleUse => true;

    [Net]
    public Player BeingUsedBy { get; private set; }

    [Net]
    public TimeUntil TimeUntilFinishedUsing { get; private set; }

    public bool IsBeingUsed => BeingUsedBy is not null;

    private static Player LocalPlayer => Game.LocalPawn as Player;

    private Vector3 EntryPosition { get; set; }

    public override void Spawn()
    {
        base.Spawn();
        Model = Model.Load("models/cinema_toilet.vmdl");
        SetupPhysicsFromModel(PhysicsMotionType.Static, false);
    }

    public bool IsUsable(Entity user)
    {
        if (user is not Player player) return false;

        return !IsBeingUsed;
    }

    public bool OnStopUse(Entity user)
    {
        Log.Info("On stopped using toilet");
        if (user is not Player player) return true;
        if (BeingUsedBy != player) return true;

        // Prevents the user from stopping use
        if (TimeUntilFinishedUsing > 0) return false;

        player.SetAnimParameter("sit", 0);
        player.BodyController.Active = true;
        player.SetParent(null);

        var toiletController = player.Components.Get<PlayerToiletController>();
        toiletController.Toilet = null;
        toiletController.Enabled = false;
        player.Position = EntryPosition;

        Log.Info("Stopped using toilet");
        BeingUsedBy = null;
        return true;
    }

    [GameEvent.Tick.Server]
    protected void Tick()
    {
        using (Prediction.Off())
        {
            if (IsBeingUsed && TimeUntilFinishedUsing < 0)
            {
                Log.Info("Timed stopped using toilet");
                BeingUsedBy.StopUsing(this);
            }
        }
    }

    public bool OnUse(Entity user)
    {
        if (user is not Player player) return false;

        if (!IsBeingUsed)
        {
            StartUsing(player);
        }

        return true;
    }

    private void StartUsing(Player player)
    {
        Log.Info("started using toilet");
        BeingUsedBy = player;
        EntryPosition = player.Position;
        TimeUntilFinishedUsing = 50;

        player.SetParent(this);
        player.LocalPosition = GetBoneTransform("seat", false).Position - player.GetBoneTransform("pelvis", false).Position;
        player.SetAnimParameter("sit", 1);
        var toiletController = player.Components.GetOrCreate<PlayerToiletController>();
        toiletController.Toilet = this;
        toiletController.Active = true;
        toiletController.Enabled = true;
    }
}
