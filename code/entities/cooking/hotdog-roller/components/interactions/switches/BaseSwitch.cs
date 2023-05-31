using Sandbox;

namespace Cinema;

public partial class BaseSwitch : EntityComponent<HotdogRoller>
{
    /// <summary>
    /// Sets switch state
    /// </summary>
    public void SetPos(int pos)
    {
        State state = (State)pos;

        TransitionStateTo(state);
    }

    /// <summary>
    /// Toggles state between State On and State Off
    /// </summary>
    public void TogglePos()
    {
        if(SwitchState == State.Off)
        {
            TransitionStateTo(State.On);

            return;
        } 
        else
        {
            TransitionStateTo(State.Off);

            return;
        }
    }

    /// <summary>
    /// Checks if state is On
    /// </summary>
    public bool IsOn()
    {
        return SwitchState == State.On;
    }
}
