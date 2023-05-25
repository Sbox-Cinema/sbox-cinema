namespace Cinema;

public partial class RightKnob
{
    /// <summary>
    ///
    /// </summary>
    protected override void HandlePosState(State state)
    {
        Log.Info($"Handling Right Knob Position State {state}");

        if (Entity is HotdogRoller hr)
        {
            hr.SetAnimParameter("RightHandleState", (int)state);
        }
    }
}
