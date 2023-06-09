using Sandbox;

namespace Cinema;
public partial class HotdogRollerKnobs : EntityComponent<HotdogRoller>
{
    private int NumKnobPositions = 7;
    private int FrontRollerKnobPosition { get; set; }
    private int BackRollerKnobPosition { get; set; }
    protected override void OnActivate()
    {
        base.OnActivate();

        FrontRollerKnobPosition = 0;
        BackRollerKnobPosition = 0;
    }
    public void SetFrontRollerKnobPos(int pos)
    {
        FrontRollerKnobPosition = pos;
    }
    public void SetBackRollerKnobPos(int pos)
    {
        BackRollerKnobPosition = pos;
    }
    public void IncrementFrontRollerKnobPos()
    {
        FrontRollerKnobPosition++;

        if(FrontRollerKnobPosition > NumKnobPositions) 
        {
            FrontRollerKnobPosition = 0;
        }
    }
    public void IncrementBackRollerKnobPos()
    {
        BackRollerKnobPosition++;

        if (BackRollerKnobPosition > NumKnobPositions)
        {
            BackRollerKnobPosition = 0;
        }
    }
    public void Simulate()
    {
        if (Game.IsClient) return;

        Entity.SetAnimParameter("RightHandleState", FrontRollerKnobPosition);
        Entity.SetAnimParameter("LeftHandleState", BackRollerKnobPosition);
    }
}
