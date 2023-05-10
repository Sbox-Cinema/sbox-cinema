using Sandbox;

namespace Cinema;

public partial class Player
{
    // <summary>
    /// Should be Input.AnalogMove
    /// </summary>
    [ClientInput]
    public Vector2 MoveInput { get; protected set; }

    /// <summary>
    /// Normalized accumulation of Input.AnalogLook
    /// </summary>
    [ClientInput]
    public Angles LookInput { get; set; }
    public Angles OriginalLookInput { get; protected set; }

    public override void BuildInput()
    {
        MoveInput = Input.AnalogMove;
        OriginalLookInput = LookInput;
        LookInput = (LookInput + Input.AnalogLook).Normal;
        ActiveChild?.AdjustInput();

        // Since we're a FPS game, let's clamp the player's pitch between -90, and 90.
        LookInput = LookInput.WithPitch(LookInput.pitch.Clamp(-90f, 90f));

    }
}
