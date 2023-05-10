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
        }
        if (Input.Pressed("slot1"))
        {
            Chair.SetAnimParameter("toggle_left_armrest", !Chair.GetAnimParameterBool("toggle_left_armrest"));
        }
        if (Input.Pressed("slot3"))
        {
            Chair.SetAnimParameter("toggle_right_armrest", !Chair.GetAnimParameterBool("toggle_right_armrest"));
        }
    }

    [ClientInput]
    public Angles LookInput { get; set; }

    public override void FrameSimulate(IClient cl)
    {
        base.FrameSimulate(cl);

        LookInput = (LookInput + Input.AnalogLook).Normal;
        LookInput = LookInput.WithPitch(LookInput.pitch.Clamp(-90f, 90f));

        Entity.EyeRotation = Entity.LookInput.ToRotation();
        var eyeAttachment = Entity.GetAttachment("eyes");
        Entity.EyeLocalPosition = Entity.Transform.PointToLocal(eyeAttachment.Value.Position);
        Entity.SimulateCamera(cl);
    }
}
