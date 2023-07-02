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

    [Net]
    public TimeSince TimeSinceStartedUsing { get; private set; }

    public enum ToiletState
    {
        Idle,
        SittingDown,
        Pooping,
        Finished
    }

    [Net]
    public ToiletState State { get; private set; } = ToiletState.Idle;

    public bool IsBeingUsed => BeingUsedBy is not null;

    private static Player LocalPlayer => Game.LocalPawn as Player;

    private Vector3 EntryPosition { get; set; }

    private static Vector3 SeatOffset => new(8, 0, 8);

    private Sound PoopingSound { get; set; }

    public enum UsageLevel
    {
        Normal,
        Big,
        Excellent
    }

    private static WeightedSoundEffect[] UsageSounds =>
        new WeightedSoundEffect[]
        {
            WeightedSoundEffect.Create().Add("sound/bodilyfunctions/normal_shit.sound", 100),
            WeightedSoundEffect.Create().Add("sound/bodilyfunctions/big_shit.sound", 100),
            WeightedSoundEffect.Create().Add("sound/bodilyfunctions/mega_shit.sound", 100)
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
        var toiletController = player.Components.GetOrCreate<PlayerToiletController>(false);
        toiletController.Toilet = this;
        toiletController.Enabled = true;
        toiletController.Active = true;
        toiletController.FreezeAim = true;
        State = ToiletState.SittingDown;
        TimeSinceStartedUsing = 0;
        ClientToiletUsed(To.Single(player));
    }

    [ClientRpc]
    private void ClientToiletUsed()
    {
        var player = Game.LocalPawn as Player;
        var game = UI.PoopGame.Instance;
        game.UseToiletCallback = (UsageLevel level) =>
        {
            ServerUseToilet(NetworkIdent, level);
        };
        player.OpenMenu(game);
    }

    [ConCmd.Server]
    private static void ServerUseToilet(int networkIdent, UsageLevel level)
    {
        var toilet = Entity.FindByIndex<Toilet>(networkIdent);
        if (toilet is null)
        {
            Log.Error($"Unable to find toilet to use with ident {networkIdent}");
            return;
        }

        toilet.UseToilet(level);
    }

    private void RemovePlayerFromToilet(Player player)
    {
        player.SetAnimParameter("sit", 0);
        player.BodyController.Active = true;
        player.SetParent(null);

        var toiletController = player.Components.GetOrCreate<PlayerToiletController>(false);
        toiletController.Toilet = null;
        toiletController.Enabled = false;
        player.Position = EntryPosition;
        State = ToiletState.Idle;
        ClientStoppedUsingToilet(To.Single(player));
    }

    [ClientRpc]
    private void ClientStoppedUsingToilet()
    {
        var player = Game.LocalPawn as Player;
        player.CloseMenu(UI.PoopGame.Instance);
    }

    public bool IsUsable(Entity user)
    {
        if (user is not Player player)
            return false;

        return !IsBeingUsed;
    }

    public bool OnStopUse(Entity user)
    {
        if (user is not Player player)
            return true;
        if (BeingUsedBy != player)
            return true;
        if (State != ToiletState.Finished)
            return false;

        RemovePlayerFromToilet(player);
        BeingUsedBy = null;
        return true;
    }

    [GameEvent.Tick.Server]
    protected void Tick()
    {
        using (Prediction.Off())
        {
            if (
                State == ToiletState.Pooping
                && TimeUntilFinishedUsing.Relative < 0
                && !PoopingSound.IsPlaying
            )
            {
                State = ToiletState.Finished;
                OnStopUse(BeingUsedBy);
            }
        }
    }

    public bool OnUse(Entity user)
    {
        if (user is not Player player)
            return false;

        if (!IsBeingUsed)
        {
            StartUsing(player);
        }

        return true;
    }

    protected void UseToilet(UsageLevel level)
    {
        State = ToiletState.Pooping;
        var playSound = async () =>
        {
            await GameTask.DelayRealtimeSeconds(0.25f);
            PoopingSound = PlaySound(UsageSounds[(int)level].GetRandom());
        };
        playSound();
        TimeUntilFinishedUsing = 5f;
    }

    private void StartUsing(Player player)
    {
        BeingUsedBy = player;
        PutPlayerOnToilet(player);
        TimeUntilFinishedUsing = 4f;
    }
}
