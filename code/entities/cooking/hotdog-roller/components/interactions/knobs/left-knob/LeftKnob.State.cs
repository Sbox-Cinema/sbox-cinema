namespace Cinema;

public partial class LeftKnob
{
    /// <summary>
    ///
    /// </summary>
    protected override void HandlePosState(State state)
    {
        Log.Info($"Handling Left Knob Position State {state}");

        HotdogRoller.SetAnimParameter("LeftHandleState", (int)state);

        Log.Info($"Left Knob Position State {HotdogRoller.GetAnimParameterInt("LeftHandleState")}");
        
    }
}
