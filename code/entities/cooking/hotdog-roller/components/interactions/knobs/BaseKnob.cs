using Sandbox;

namespace Cinema;

public partial class BaseKnob : EntityComponent
{
    public void SetPos(int pos)
    {
        State state = (State)pos;

        TransitionStateTo(state);
    }

    public void IncrementPos()
    {
        KnobState++;

        if((int)KnobState > (int)State.PosSeven)
        {
            TransitionStateTo(State.PosZero);
        } 
        else
        {
            TransitionStateTo(KnobState);
        }
    }

   
}
