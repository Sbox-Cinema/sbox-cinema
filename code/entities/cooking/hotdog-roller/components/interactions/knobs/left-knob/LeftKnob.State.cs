namespace Cinema;

public partial class LeftKnob
{
    /// <summary>
    /// Overrides base to handle left knob state
    /// </summary>
    protected override void HandlePosState(State state)
    {
        Entity.SetAnimParameter("LeftHandleState", (int) state);
    }
}
