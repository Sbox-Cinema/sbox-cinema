using Sandbox;

namespace Cinema;

public partial class Switch : EntityComponent
{
    private HotdogRoller Machine => Entity as HotdogRoller;
    public enum Side : int
    {
        Left,
        Right
    }

    public Side SwitchSide { get; set; }

    public Switch()
    {

    }
    public Switch(Side side)
    {
        SwitchSide = side;
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
