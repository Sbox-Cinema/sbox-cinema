namespace Cinema;

public partial class RightKnob
{
    /// <summary>
    /// Overrides base to handle right knob state
    /// </summary>
    protected override void HandlePosState(State state)
    {
        Entity.SetAnimParameter("RightHandleState", (int) state);
    }
}
