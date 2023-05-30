namespace Cinema;

public partial class RightKnob
{
    /// <summary>
    ///
    /// </summary>
    protected override void HandlePosState(State state)
    {
        Entity.SetAnimParameter("RightHandleState", (int) state);
    }
}
