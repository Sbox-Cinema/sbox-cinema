using Sandbox;

namespace Cinema;

public partial class PlayerToiletController : PlayerController
{

    [Net]
    public Toilet Toilet { get; set; }

    protected override void OnActivate()
    {
        base.OnActivate();

        SinceActivated = 0f;
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

    public override void FrameSimulate(IClient cl)
    {
        base.FrameSimulate(cl);

        Entity.SimulateCamera(cl);

        LookInput = (LookInput + Input.AnalogLook).Normal;
        LookInput = LookInput.WithPitch(LookInput.pitch.Clamp(-90f, 90f));

        var eyeOffset = new Vector3(-6, 0, 40);
        LookPosition = Entity.Transform.PointToWorld(eyeOffset);
    }
}
