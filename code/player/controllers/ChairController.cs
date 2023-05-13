using Sandbox;

namespace Cinema;

public partial class ChairController : PlayerController
{
    /// <summary>
    /// Enables spawning cups in the left and right armrests by
    /// pressing <c>slot3</c> and <c>slot4</c> respectively.
    /// </summary>
    [ConVar.Replicated("cinema.chair.debug")]
    public static bool ChairDebug { get; set; } = false;

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

        HandleArmrest(Armrest.Sides.Left, "slot1", "slot3");
        HandleArmrest(Armrest.Sides.Right, "slot2", "slot4");
    }

    private void HandleArmrest(Armrest.Sides side, string toggleSlot, string debugSlot)
    {
        if (Input.Pressed(toggleSlot))
        {
            Chair.Armrests[side]?.Toggle();
            return;
        }
        if (ChairDebug && Input.Pressed(debugSlot))
        {
            PrimeTest(side);
        }
    }

    private void PrimeTest(Armrest.Sides side)
    {
        var cup = new ModelEntity("models/papercup/papercup.vmdl");
        cup.Tags.Add("solid");
        cup.SetupPhysicsFromModel(PhysicsMotionType.Dynamic);
        Chair.Armrests[side]?.TryHoldEntity(cup);
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

        var eyeAttachment = Entity.GetAttachment("eyes");
        LookPosition = eyeAttachment.Value.Position;

        if (ChairDebug && Chair.IsValid())
        {
            var leftArmrest = Chair.LeftArmrest;
            var rightArmrest = Chair.RightArmrest;
            DebugOverlay.ScreenText(
                text: $"Left Armrest - State: {leftArmrest?.State.ToString() ?? "null"}, Entity: {leftArmrest?.HeldEntity?.Name ?? "null"}",
                line: 0
                );
            DebugOverlay.ScreenText(
                text: $"Right Armrest: {rightArmrest?.State.ToString() ?? "null"}, Entity: {rightArmrest?.HeldEntity?.Name ?? "null"}",
                line: 1
                );
        }
    }
}
