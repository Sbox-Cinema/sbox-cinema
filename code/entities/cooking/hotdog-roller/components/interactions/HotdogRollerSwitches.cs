using Sandbox;

namespace Cinema;

public partial class HotdogRollerSwitches : EntityComponent<HotdogRoller>
{
    private enum IndicatorLightGroup : int
    {
        BothOff,
        BothOn,
        FrontOn,
        BackOn,
    }
    private bool FrontRollerPowerOn { get; set; } = false;
    private bool BackRollerPowerOn { get; set; } = false;
    private IndicatorLightGroup IndicatorLight => GetIndicatorLightGroup();

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
    public void Simulate()
    {
        if (Game.IsClient) return;

        // Update Switch Animation
        Entity.SetAnimParameter("toggle_right", FrontRollerPowerOn);
        Entity.SetAnimParameter("toggle_left", BackRollerPowerOn);

        // Update Power Light Indicator Material
        Entity.SetMaterialGroup((int)IndicatorLight);
    }
    private IndicatorLightGroup GetIndicatorLightGroup()
    {
        if (FrontRollerPowerOn && BackRollerPowerOn)
        {
            return IndicatorLightGroup.BothOn;
        }

        else if (FrontRollerPowerOn && !BackRollerPowerOn)
        {
            return IndicatorLightGroup.FrontOn;
        }

        else if (!FrontRollerPowerOn && BackRollerPowerOn)
        {
            return IndicatorLightGroup.BackOn;
        }

        return IndicatorLightGroup.BothOff;
    }
}
