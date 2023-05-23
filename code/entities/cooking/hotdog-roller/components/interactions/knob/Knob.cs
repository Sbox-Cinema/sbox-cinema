using Sandbox;

namespace Cinema;

public partial class Knob : EntityComponent
{
    private HotdogRoller Machine =>  Entity as HotdogRoller;
    public enum Side : int
    {
        Left,
        Right
    }
    public Side KnobSide { get; set; }

    public Knob()
    {

    }
    public Knob(Side side)
    {
        KnobSide = side;
    }

    public void SetPos(int pos)
    {
        State state = (State)pos;

        TransitionStateTo(state);
    }

    protected override void OnActivate()
    {
        base.OnActivate();

        SetDefaultState();
    }
    protected override void OnDeactivate()
    {
        base.OnDeactivate();
    }

    private void SetDefaultState()
    {
        SetPos(0);
    }

    
}
