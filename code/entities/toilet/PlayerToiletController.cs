using Sandbox;

namespace Cinema;

public partial class PlayerToiletController : PlayerController
{

    [Net]
    public Toilet Toilet { get; set; }

    protected override void OnActivate()
    {
        base.OnActivate();

        Log.Info("PlayerToiletController activated");

        Entity.LookInput = Toilet.Rotation.Angles();
        Entity.DrawHead(false);
        Entity.SetDrawTaggedClothing("Head", false);

        SinceActivated = 0f;
    }

    protected override void OnDeactivate()
    {
        base.OnDeactivate();

        Log.Info("PlayerToiletController deactivated");

        Entity.DrawHead(true);
        Entity.SetDrawTaggedClothing("Head", true);
    }

    protected TimeSince SinceActivated { get; set; }

    public override void Simulate(IClient cl)
    {
        base.Simulate(cl);
        Entity.Rotation = Toilet.Rotation;

        Entity.EyeRotation = Entity.LookInput.ToRotation();
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
    public Angles LookInput { get; set; }

    [ClientInput]
    public Vector3 LookPosition { get; set; }

    private Vector3 EyeOffset => new(0, 0, 50);

    public override void FrameSimulate(IClient cl)
    {
        base.FrameSimulate(cl);

        //Entity.SimulateCamera(cl);
        SimulateCamera(cl);

        LookInput = (LookInput + Input.AnalogLook).Normal;
        LookInput = LookInput.WithPitch(LookInput.pitch.Clamp(-90f, 90f));

        LookPosition = Entity.Transform.PointToWorld(EyeOffset);
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
