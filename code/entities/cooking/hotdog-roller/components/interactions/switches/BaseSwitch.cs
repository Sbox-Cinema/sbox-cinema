using Sandbox;

namespace Cinema;

public partial class BaseSwitch : EntityComponent<HotdogRoller>
{
    public void SetPos(int pos)
    {
        State state = (State)pos;

        TransitionStateTo(state);
    }

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

    public bool IsOn()
    {
        return SwitchState == State.On;
    }
}
