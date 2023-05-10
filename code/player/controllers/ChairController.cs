using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema;

public partial class ChairController : PlayerController
{
    [Net]
    public CinemaChair Chair { get; set; }

    protected override void OnActivate()
    {
        base.OnActivate();

        SinceActivated = 0f;
    }

    protected TimeSince SinceActivated { get; set; }

    public override void Simulate(IClient cl)
    {
        base.Simulate(cl);
        Entity.Rotation = Chair.Rotation;

        Entity.EyeRotation = Entity.LookInput.ToRotation();
        Entity.EyeLocalPosition = Entity.Transform.PointToLocal(LookPosition);

        SimulateAnimation();

        if (Game.IsClient)
        {
            return;
        }

        // Wait a while after activing to check whether the use button is pressed,
        // as the input to begin using the chair may have occurred on the same tick.
        if (Input.Pressed("use") && SinceActivated > Time.Delta)
        {
            Log.Trace($"{Entity.Client} - Stopped sitting in chair: {Chair.Name}");
            Chair.EjectUser();
            return;
        }
        if (Input.Pressed("slot1"))
        {
            Chair.ToggleArmrest(CinemaChair.ArmrestSide.Left);
        }
        if (Input.Pressed("slot3"))
        {
            Chair.ToggleArmrest(CinemaChair.ArmrestSide.Right);
        }
    }

    public void SimulateAnimation()
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

        var eyeAttachment = Entity.GetAttachment("eyes");
        LookPosition = eyeAttachment.Value.Position;
    }
}
