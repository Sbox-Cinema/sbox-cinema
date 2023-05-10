using Editor;
using Sandbox;
using Sandbox.Diagnostics;

namespace Cinema;

[Library("ent_cinemachair"), HammerEntity]
[Model(Archetypes = ModelArchetype.animated_model, MaterialGroup = "Deep_Green_Suede", Model = "models/cinemachair/cinemachair01.vmdl")]
[Title("Cinema Chair"), Category("Gameplay"), Icon("event_seat")]
public partial class CinemaChair : AnimatedEntity, ICinemaUse
{
    public bool IsOccupied => Occupant.IsValid();

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
        if (!user.IsValid)
        {
            return false;
        }
        // TODO: Check whether player is in front of seat.
        return !IsOccupied;
    }

    public bool OnUse(Entity user)
    {
        Log.Trace($"{user.Client} - Began sitting in chair: {Name}");

        Occupant = user as Player;

        Assert.NotNull(Occupant);

        Occupant.SetParent(this);
        Occupant.LocalPosition = SeatOffset;
        Occupant.SetAnimParameter("sit", 1);
        Occupant.ShouldUpdateAnimation = false;
        Occupant.ShouldUpdateUse = false;
        Occupant.ShouldUpdateCamera = false;

        var chairComponent = Occupant.Components.GetOrCreate<ChairController>();
        chairComponent.Chair = this;
        chairComponent.Active = true;
        chairComponent.Enabled = true;

        SetAnimParameter("toggle_seat", true);

        return false;
    }

    public void OnStopUse(Entity user)
    {

    }

    public void EjectUser()
    {
        if (Occupant == null)
        {
            Log.Error($"{Name} Cannot eject occupant from unoccupied chair.");
            return;
        }

        Occupant.SetParent(null);
        Occupant.Position = Transform.PointToWorld(EjectOffset);
        Occupant.BodyController.Active = true;
        var chairComponent = Occupant.Components.Get<ChairController>();
        chairComponent.Chair = null;
        chairComponent.Enabled = false;
        Occupant.SetAnimParameter("sit", 0);
        Occupant.ShouldUpdateAnimation = true;
        Occupant.ShouldUpdateCamera = true;
        Occupant.ShouldUpdateUse = true;
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
