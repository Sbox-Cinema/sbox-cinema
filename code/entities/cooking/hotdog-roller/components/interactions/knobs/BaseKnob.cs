using Sandbox;

namespace Cinema;

public partial class BaseKnob : EntityComponent<HotdogRoller>
{
    [Net] public HotdogRoller HotdogRoller { get; set; }

    protected override void OnActivate()
    {
        base.OnActivate();

        HotdogRoller = Entity as HotdogRoller;
    }
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
