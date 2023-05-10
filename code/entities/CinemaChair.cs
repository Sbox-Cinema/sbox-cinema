using Editor;
using Sandbox;
using Sandbox.Diagnostics;

namespace Cinema;

[Library("ent_cinemachair"), HammerEntity]
[Model(Archetypes = ModelArchetype.animated_model, MaterialGroup = "Deep_Green_Suede", Model = "models/cinemachair/cinemachair01.vmdl")]
[Title("Cinema Chair"), Category("Gameplay"), Icon("event_seat")]
public partial class CinemaChair : AnimatedEntity, ICinemaUse
{
    public bool IsOccupied => Occupant != null && Occupant.IsValid();

    [Net]
    public Player Occupant { get; protected set; }

    /// <summary>
    /// An offset from the origin of this chair that shall be used to determine the position 
    /// of an entity sitting in this chair.
    /// </summary>
    [Property]
    public Vector3 SeatOffset { get; set; } = new Vector3(0, 0, 0);

    /// <summary>
    /// An offset from the origin of this chair that shall be used to determine the position
    /// at which an entity will be placed after it has stopped sitting in this chair.
    /// </summary>
    [Property]
    public Vector3 EjectOffset { get; set; }

    [Property]
    public Vector3 EyeOffset { get; set; } = new Vector3(0, 0, 64);

    public string UseText => "Sit Down";
    public string CannotUseText
    {
        get
        {
            if (IsOccupied)
            {
                return "Seat is occupied.";
            }
            else
            {
                return null;
            }
        }
    }

    public override void Spawn()
    {
        base.Spawn();

        Tags.Add("solid", "chair");

        var capsule = new Capsule(
            Vector3.Zero,
            new Vector3(0, 0, 64),
            16f
            );
        SetupPhysicsFromCapsule(PhysicsMotionType.Keyframed, capsule);
        ResetAnimParameters();
    }

    public bool IsUsable(Entity user)
    {
        if (user == null || !user.IsValid)
        {
            return false;
        }
        // Add another check for whether this is an assigned/VIP seat?
        return !IsOccupied;
    }

    public void OnStopUse(Entity user)
    {
        Log.Info($"{user.Client} - Stopped using chair: {Name}");
    }

    public bool OnUse(Entity user)
    {
        Log.Info($"{user.Client} - Began using chair: {Name}");

        Occupant = user as Player;

        Assert.NotNull(Occupant);

        user.Client.Pawn = this;

        Occupant.SetParent(this);
        Occupant.LocalPosition = SeatOffset;
        Occupant.SetAnimParameter("sit", 1);

        SetAnimParameter("toggle_seat", true);

        return false;
    }

    public override void Simulate(IClient cl)
    {
        base.Simulate(cl);
        Occupant.Rotation = Rotation;

        if (Game.IsClient)
        {
            SimulateCamera(cl);
            return;
        }

        if (Input.Pressed("use"))
        {
            EjectUser();
        }
    }

    public void SimulateCamera(IClient cl)
    {
        var eyeAttachment = Occupant?.GetAttachment("eyes");
        Camera.Position = eyeAttachment?.Position ?? Transform.PointToWorld(EyeOffset);
        Camera.Rotation = LookInput.ToRotation();
    }

    [ClientInput]
    public Angles LookInput { get; set; }

    public override void BuildInput()
    {
        base.BuildInput();

        LookInput = (LookInput + Input.AnalogLook).Normal;
        LookInput = LookInput.WithPitch(LookInput.pitch.Clamp(-90f, 90f));
    }

    public void EjectUser()
    {
        if (Occupant == null)
        {
            Log.Info($"{Name} Cannot eject occupant from unoccupied chair.");
            return;
        }

        Occupant.SetParent(null);
        Occupant.Position = Transform.PointToWorld(EjectOffset);
        Occupant.Client.Pawn = Occupant;
        Occupant = null;

        SetAnimParameter("toggle_seat", false);
    }

    public static void DrawGizmos(EditorContext context)
    {
        if (!context.IsSelected)
        {
            return;
        }
        var offsetProp = context.Target.GetProperty("SeatOffset");
        var offset = offsetProp.As.Vector3;

        Gizmo.Draw.Color = new Color(0f, 0.2f, 0.8f, 1f);
        Gizmo.Draw.SolidSphere(offset, 1f);

        var ejectProp = context.Target.GetProperty("EjectOffset");
        var eject = ejectProp.As.Vector3;

        Gizmo.Draw.Color = new Color(0.8f, 0.1f, 0.1f, 1f);
        Gizmo.Draw.SolidSphere(eject, 1f);
    }
}
