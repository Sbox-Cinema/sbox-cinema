namespace Cinema;

public partial class LeftKnob
{
    /// <summary>
    ///
    /// </summary>
    protected override void HandlePosState(State state)
    {
        Entity.SetAnimParameter("LeftHandleState", (int) state);
    }
}
