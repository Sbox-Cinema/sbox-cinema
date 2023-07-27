using Sandbox;

namespace Cinema;

public partial class PlayerToiletController : PlayerController
{
    [Net]
    public Toilet Toilet { get; set; }

    [Net]
    public bool FreezeAim { get; set; }

    public Rotation FrozenAimRotation
    {
        get
        {
            var angles = Toilet.Rotation.Angles();
            angles.pitch += 45;
            return angles.ToRotation();
        }
    }

    protected override void OnActivate()
    {
        base.OnActivate();

        if (Toilet is null)
            return;

        Log.Info("PlayerToiletController activated");

        Entity.LookInput = Toilet.Rotation.Angles();
        Entity.DrawHead(false);
        Entity.SetDrawTaggedClothing("Hat", false);
        Entity.SetDrawTaggedClothing("Hair", false);
        Entity.SetDrawTaggedClothing("Bottoms", false);

        SinceActivated = 0f;
    }

    protected override void OnDeactivate()
    {
        base.OnDeactivate();

        Log.Info("PlayerToiletController deactivated");

        Entity.DrawHead(true);
        Entity.SetDrawTaggedClothing("Hat", true);
        Entity.SetDrawTaggedClothing("Hair", true);
        Entity.SetDrawTaggedClothing("Bottoms", true);
    }

    protected TimeSince SinceActivated { get; set; }

    public override void Simulate(IClient cl)
    {
        base.Simulate(cl);
        Entity.Rotation = Toilet.Rotation;

        if (FreezeAim)
        {
            Entity.EyeRotation = FrozenAimRotation;
        }
        else
        {
            Entity.EyeRotation = Entity.LookInput.ToRotation();
        }

        Entity.EyeLocalPosition = Entity.Transform.PointToLocal(LookPosition);

        SimulateAnimation();

        if (Game.IsClient)
        {
            return;
        }
    }

    private void SimulateAnimation()
    {
        var aimPos = Entity.AimRay.Position + Entity.EyeRotation.Forward * 128.0f;

        var localPos = new Transform(Entity.AimRay.Position, Entity.Rotation).PointToLocal(aimPos);

        Entity.SetAnimParameter("aim_eyes", localPos);
        Entity.SetAnimParameter("aim_head", localPos);
        Entity.SetAnimParameter("aim_body", localPos);
    }

    [ClientInput]
    public Vector3 LookPosition { get; set; }

    private Vector3 EyeOffset => new(9, 0, 2);

    private static float PitchLock => 70f;
    private static float YawLock => 40;

    public override void FrameSimulate(IClient cl)
    {
        base.FrameSimulate(cl);

        Entity.LookInput = Entity.LookInput.WithPitch(
            Entity.LookInput.pitch.Clamp(-PitchLock, PitchLock)
        );

        var lookYaw = Angles.NormalizeAngle(Entity.LookInput.yaw - Toilet.Rotation.Yaw());
        lookYaw = lookYaw.Clamp(-YawLock, YawLock);
        Entity.LookInput = Entity.LookInput.WithYaw(Toilet.Rotation.Yaw() + lookYaw);

        LookPosition = Entity.Transform.PointToWorld(
            Entity.GetBoneTransform("head", false).Position + EyeOffset
        );

        Entity.EyeRotation = Entity.LookInput.ToRotation();
        if (FreezeAim)
        {
            Entity.EyeRotation = FrozenAimRotation;
            Entity.LookInput = FrozenAimRotation.Angles();
        }

        Entity.EyeLocalPosition = Entity.Transform.PointToLocal(LookPosition);

        SimulateCamera(cl);
    }

    protected void SimulateCamera(IClient cl)
    {
        Camera.Rotation = Entity.EyeRotation;

        // Set field of view to whatever the user chose in options
        Camera.FieldOfView = Screen.CreateVerticalFieldOfView(Game.Preferences.FieldOfView);

        Camera.Position = Entity.EyePosition;
        Camera.ZNear = 0.5f;

        // Set the first person viewer to this, so it won't render our model
        Camera.FirstPersonViewer = null;
    }
}
