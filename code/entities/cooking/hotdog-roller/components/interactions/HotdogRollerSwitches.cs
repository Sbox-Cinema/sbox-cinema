using Sandbox;

namespace Cinema;

public partial class HotdogRollerSwitches : EntityComponent<HotdogRoller>
{
    private bool FrontRollerPowerOn { get; set; }
    private bool BackRollerPowerOn { get; set; }

    protected override void OnActivate()
    {
        base.OnActivate();
        
        FrontRollerPowerOn = false;
        BackRollerPowerOn = false;
    }

    protected override void OnDeactivate()
    {
        base.OnDeactivate();

        FrontRollerPowerOn = false;
        BackRollerPowerOn = false;
    }

    public void ToggleFrontRollerPower()
    {
        FrontRollerPowerOn = !FrontRollerPowerOn;
    }

    public void ToggleBackRollerPower()
    {
        BackRollerPowerOn = !BackRollerPowerOn;
    }

    public bool IsFrontRollerPoweredOn()
    {
        return FrontRollerPowerOn;
    }

    public bool IsBackRollerPoweredOn()
    {
        return BackRollerPowerOn;
    }

    [GameEvent.Tick] 
    private void OnTick()
    {
        Entity.SetAnimParameter("toggle_right", FrontRollerPowerOn);
        Entity.SetAnimParameter("toggle_left", BackRollerPowerOn);
    }
}
