using Sandbox;

namespace Cinema;

public partial class HotdogRoller
{
    /// <summary>
    ///
    /// </summary>
    private void TogglePower()
    {
        if(MachineState == State.On)
        {
            TransitionStateTo(State.Off);

            return;
        }

        if(MachineState == State.Off)
        {
            TransitionStateTo(State.On);

            return;
        }
    }
}
