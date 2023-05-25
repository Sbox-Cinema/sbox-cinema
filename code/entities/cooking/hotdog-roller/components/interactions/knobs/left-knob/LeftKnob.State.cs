using System.Reflection.PortableExecutable;

namespace Cinema;

public partial class LeftKnob
{
    /// <summary>
    ///
    /// </summary>
    protected override void HandlePosState(State state)
    {
        Log.Info($"Handling Left Knob Position State {state}");

        if (Entity is HotdogRoller hr)
        {
            hr.SetAnimParameter("LeftHandleState", (int)state);

            Log.Info($"Left Knob Position State {hr.GetAnimParameterInt("LeftHandleState")}");
        }
    }
}
