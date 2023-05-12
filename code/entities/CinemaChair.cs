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
        ResetAnimParameters();
    }

    [GameEvent.Client.Frame]
    public void OnFrame()
    {
        if (!ChairDebug || Occupant == null)
        {
            return;
        }

        var left = GetCupholderTransform(ArmrestSide.Left);
        DebugOverlay.Sphere(left.Position, 3f, Color.Red);
        var right = GetCupholderTransform(ArmrestSide.Right);
        DebugOverlay.Sphere(right.Position, 3f, Color.Red);
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
    public Entity LeftCuphold { get; set; }
    [Net]
    public Entity RightCuphold { get; set; }

    // CW: Cupholdry
    public void CupholdEntity(ArmrestSide side, Entity entity)
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

    public async void LaunchCuphold(ArmrestSide side)
    {
        Entity entity;
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
        if (entity == null)
        {
            return;
        }
        if (!entity.IsValid())
        {
            Log.Error($"{Name} - No entity in {side} cupholder.");
            return;
        }
        entity.SetParent(null);
        entity.ApplyLocalImpulse(Vector3.Up * 250f);
        await Task.Delay(100);
        entity.ApplyLocalAngularImpulse(Vector3.Random * 500f);
    }

    public async void ToggleArmrest(ArmrestSide side)
    {
        var paramName = side switch
        {
            ArmrestSide.Left => "toggle_left_armrest",
            ArmrestSide.Right => "toggle_right_armrest",
            _ => throw new System.Exception("What the fuck did you just fucking say about me, you little bitch? I'll have you know I graduated top of my class in the Navy Seals, and I've been involved in numerous secret raids on Al-Quaeda, and I have over 300 confirmed kills. I am trained in gorilla warfare and I'm the top sniper in the entire US armed forces. You are nothing to me but just another target. I will wipe you the fuck out with precision the likes of which has never been seen before on this Earth, mark my fucking words. You think you can get away with saying that shit to me over the Internet? Think again, fucker. As we speak I am contacting my secret network of spies across the USA and your IP is being traced right now so you better prepare for the storm, maggot. The storm that wipes out the pathetic little thing you call your life. You're fucking dead, kid. I can be anywhere, anytime, and I can kill you in over seven hundred ways, and that's just with my bare hands. Not only am I extensively trained in unarmed combat, but I have access to the entire arsenal of the United States Marine Corps and I will use it to its full extent to wipe your miserable ass off the face of the continent, you little shit. If only you could have known what unholy retribution your little \"clever\" comment was about to bring down upon you, maybe you would have held your fucking tongue. But you couldn't, you didn't, and now you're paying the price, you goddamn idiot. I will shit fury all over you and you will drown in it. You're fucking dead, kiddo.")
        };
        var originalValue = GetAnimParameterBool(paramName);
        SetAnimParameter(paramName, !originalValue);
        // If the armrest was originally down, we're raising it now, so launch the contents.
        if (!originalValue)
        {
            // Hack, wait until the the animation is nearly finished. 
            // Should probably use animation events instead
            await Task.Delay(600);
            LaunchCuphold(side);
        }
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
