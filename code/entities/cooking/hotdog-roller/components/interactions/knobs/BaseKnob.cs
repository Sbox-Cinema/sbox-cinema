using Sandbox;

namespace Cinema;

public partial class BaseKnob : EntityComponent<HotdogRoller>
{
    /// <summary>
    /// Handles knob state
    /// </summary>
    protected override void OnActivate()
    {
        base.OnActivate();

        TransitionStateTo(State.Zero);
    }

    /// <summary>
    /// Sets knob state 
    /// </summary>
    public void SetPos(int pos)
    {
        State state = (State) pos;

        TransitionStateTo(state);
    }

    /// <summary>
    /// Increments knob state
    /// </summary>
    public void IncrementPos()
    {
        KnobState++;

        if((int) KnobState > (int) State.Seven)
        {
            TransitionStateTo(State.Zero);
        } 
        else
        {
            TransitionStateTo(KnobState);
        }
    }
}
