namespace Cinema;

public partial class RightKnob
{
    /// <summary>
    ///
    /// </summary>
    protected override void HandlePosState(State state)
    {
        Log.Info($"Handling Right Knob Position State {state}");

        HotdogRoller.SetAnimParameter("RightHandleState", (int)state);

        Log.Info($"Right Knob Position State {HotdogRoller.GetAnimParameterInt("RightHandleState")}");
    }
}
