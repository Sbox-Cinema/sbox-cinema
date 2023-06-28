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

    private static Vector3 SeatOffset => new(8, 0, 8);

    public enum UsageLevel
    {
        Normal,
        Big
    }

    private readonly WeightedSoundEffect[] UsageSounds = new WeightedSoundEffect[]{
        WeightedSoundEffect.Create().Add("sound/bodilyfunctions/normal_shit.sound", 100),
        WeightedSoundEffect.Create().Add("sound/bodilyfunctions/big_shit.sound", 900).Add("sound/bodilyfunctions/mega_shit.sound", 100)
    };

    public override void Spawn()
    {
        base.Spawn();
        Model = Model.Load("models/cinema_toilet.vmdl");
        SetupPhysicsFromModel(PhysicsMotionType.Static, false);
    }

    private void PutPlayerOnToilet(Player player)
    {
        EntryPosition = player.Position;
        player.SetParent(this);
        player.LocalPosition = SeatOffset;
        player.SetAnimParameter("sit", 1);
        var toiletController = player.Components.GetOrCreate<PlayerToiletController>();
        toiletController.Toilet = this;
        toiletController.Active = true;
        toiletController.Enabled = true;
    }

    private void RemovePlayerFromToilet(Player player)
    {
        player.SetAnimParameter("sit", 0);
        player.BodyController.Active = true;
        player.SetParent(null);

        var toiletController = player.Components.GetOrCreate<PlayerToiletController>();
        toiletController.Toilet = null;
        toiletController.Enabled = false;
        player.Position = EntryPosition;
    }

    public bool IsUsable(Entity user)
    {
        if (user is not Player player) return false;

        return !IsBeingUsed;
    }

    public bool OnStopUse(Entity user)
    {
        if (user is not Player player) return true;
        if (BeingUsedBy != player) return true;

        // Prevents the user from stopping use early
        if (TimeUntilFinishedUsing > 0) return false;

        RemovePlayerFromToilet(player);
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
        BeingUsedBy = player;
        PutPlayerOnToilet(player);
        TimeUntilFinishedUsing = 3;
        player.PlaySound(UsageSounds[(int)UsageLevel.Normal].GetRandom());
    }
}
