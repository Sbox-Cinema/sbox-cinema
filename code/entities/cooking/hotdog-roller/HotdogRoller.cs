using Editor;
using Sandbox;

namespace Cinema;

[Library("ent_hotdog_roller"), HammerEntity]
[Title("Hotdog Roller"), Category("Gameplay"), Icon("monitor")]
[EditorModel("models/hotdogroller/hotdogroller.vmdl")]
[SupportsSolid]
public partial class HotdogRoller : AnimatedEntity, IUse
{
    public UI.Tooltip Tooltip { get; set; }

    public override void Spawn()
    {
        base.Spawn();

        Transmit = TransmitType.Always;

        SetModel("models/hotdogroller/hotdogroller.vmdl");

        SetupPhysicsFromModel(PhysicsMotionType.Keyframed);

        Tags.Add("interactable");
    }

    public override void ClientSpawn()
    {
        base.ClientSpawn();

        SetupUI();
    }

    /// <summary>
    /// Sets up the UI when the Hotdog roller is interacted with
    /// </summary>
    private void SetupUI()
    {
        Tooltip = new UI.Tooltip("Press E to use machine");

        Tooltip.Transform = Transform;
        Tooltip.Position += Vector3.Up * 32.0f;
    }

    public override void Simulate(IClient cl)
    {
        base.Simulate(cl);

        if (Game.IsServer)
        {
            using (Prediction.Off())
            {
                if (Input.Pressed(InputButton.Use))
                {
                    Log.Info("turn off??");
                }
            }

        }
    }

    /// <summary>
    /// Whether this Hotdog Roller is usable or not
    /// </summary>
    /// <param name="user">The player who is using</param>
    /// <returns>If this is useable</returns>
    public virtual bool IsUsable(Entity user)
    {
        return true;
    }

    /// <summary>
    /// Called on the server when the Hotdog Roller is used by a player
    /// </summary>
    /// <param name="user"></param>
    /// <returns>If the player can continue to use the Hotdog Roller</returns>
    public virtual bool OnUse(Entity user)
    {
        if (user is not Player player) return false;

        return false;
    }

    [GameEvent.Tick]
    public void Tick()
    {
        if (Game.LocalPawn is Player player)
        {
            TraceResult tr = Trace.Ray(player.AimRay, 1024)
                .WithTag("interactable")
                .Ignore(player)
                .Run();

            if (tr.Hit)
            {
                Tooltip.ShouldOpen(true);
            }
            else
            {
                Tooltip.ShouldOpen(false);
            }

            Tooltip.Rotation = Rotation.LookAt(Camera.Rotation.Forward * -1.0f, Vector3.Up);
        }
    }
}
