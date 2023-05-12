using Editor;
using Sandbox;
using Sandbox.Diagnostics;

namespace Cinema;

[Library("ent_cinemachair"), HammerEntity]
[Model(Archetypes = ModelArchetype.animated_model, MaterialGroup = "Deep_Green_Suede", Model = "models/cinemachair/cinemachair01.vmdl")]
[Title("Cinema Chair"), Category("Gameplay"), Icon("event_seat")]
public partial class CinemaChair : AnimatedEntity, ICinemaUse
{
    [ConVar.Replicated("cinema.chair.debug")]
    public static bool ChairDebug { get; set; } = false;

    public enum ArmrestSide
    {
        Left,
        Right
    }

    public bool IsOccupied => Occupant.IsValid();

    [Net]
    public Player Occupant { get; protected set; }

    /// <summary>
    /// An offset from the origin of this chair that shall be used to determine the position 
    /// of an entity sitting in this chair.
    /// </summary>
    [Net, Property]
    public Vector3 SeatOffset { get; set; } = new Vector3(0, 0, 0);

    /// <summary>
    /// An offset from the origin of this chair that shall be used to determine the position
    /// at which an entity will be placed after it has stopped sitting in this chair.
    /// </summary>
    [Net, Property]
    public Vector3 EjectOffset { get; set; }

    /// <summary>
    /// Specifies the angles from which this chair is usable. Based on
    /// the dot product of the chair's forward direction and the direction 
    /// vector between the player and the chair. For example,
    /// 
    ///     -1:     Usable from any direction
    ///     0:      Usable from the size or front
    ///     0.5:    Usable from the front
    /// </summary>
    [Net, Property]
    public float UseAngle { get; set; } = 0.7f;

    [Net, Property]
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
    }

    public bool IsUsable(Entity user)
    {
        if (!user.IsValid)
        {
            return false;
        }
        var direction = (user.Position - Position).Normal;
        var angle = direction.Dot(Rotation.Forward);
        Log.Trace($"Chair use angle: {angle}, Min valid angle: {UseAngle}");
        // Check whether the player is in front of the seat.
        if (angle < UseAngle)
        {
            return false;
        }
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
        // NOP, required by the interface.
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

    [Net]
    public ModelEntity LeftCuphold { get; set; }
    [Net]
    public ModelEntity RightCuphold { get; set; }

    public void CupholdEntity(ArmrestSide side, ModelEntity entity)
    {
        string boneName = GetCupholderBoneName(side);
        if (side == ArmrestSide.Left)
        {
            LeftCuphold = entity;
        }
        else
        {
            RightCuphold = entity;
        }
        entity.Transform = GetBoneTransform(boneName).WithRotation(Rotation.Identity);
        entity.SetParent(this, boneName);
    }

    private string GetCupholderBoneName(ArmrestSide side)
        => side switch
        {
            ArmrestSide.Left => "cupholder_l",
            ArmrestSide.Right => "cupholder_r",
            _ => throw new System.Exception(">:(")
        };

    private Transform GetCupholderTransform(ArmrestSide side)
        => GetBoneTransform(GetCupholderBoneName(side));

    protected override void OnAnimGraphTag(string tag, AnimGraphTagEvent fireMode)
    {
        base.OnAnimGraphTag(tag, fireMode);

        if (Game.IsClient)
        {
            return;
        }
        if (fireMode is not AnimGraphTagEvent.Fired or AnimGraphTagEvent.Start)
        {
            return;
        }

        // When the left or right armrest is fully raised, we'd like to launch
        // the corresponding cupholded entity.
        if (tag == "launch_left_armrest")
        {
            LaunchCuphold(ArmrestSide.Left);
            return;
        }
        else if (tag == "launch_right_armrest")
        {
            LaunchCuphold(ArmrestSide.Right);
            return;
        }
    }

    public async void LaunchCuphold(ArmrestSide side)
    {
        ModelEntity entity;
        if (side == ArmrestSide.Left)
        {
            entity = LeftCuphold;
            LeftCuphold = null;
        }
        else
        {
            entity = RightCuphold;
            RightCuphold = null;
        }
        if (!entity.IsValid())
        {
            Log.Trace($"{Name} - No entity in {side} cupholder.");
            return;
        }
        // Make sure the cup doesn't collide with the arm rest for now.
        entity.EnableSolidCollisions = false;
        entity.SetParent(null);
        var launchAngle = (Rotation.Backward + Rotation.Up).Normal;
        entity.ApplyAbsoluteImpulse(launchAngle * 250f);
        entity.ApplyLocalAngularImpulse(Vector3.Random * 1000f);
        // Give the cup some time to fly away from the chair.
        await GameTask.Delay(200);
        entity.EnableSolidCollisions = true;
    }

    public async void ToggleArmrest(ArmrestSide side)
    {
        var paramName = side switch
        {
            ArmrestSide.Left    => "toggle_left_armrest",
            _                   => "toggle_right_armrest"
        };
        var originalValue = GetAnimParameterBool(paramName);
        SetAnimParameter(paramName, !originalValue);
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
