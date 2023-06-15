using Sandbox;
using Cinema.Interactables;

namespace Cinema.HotdogRoller;

public class Switch : BaseInteractable
{
    private string animName { get; set; }
    public bool TogglePower { get; set; }

    public Switch() // For the compiler...
    {
    }

    public Switch(string animName)
    {
        this.animName = animName;
    }

    public override void Trigger()
    {
        var parent = Parent as AnimatedEntity;
        var hotdogRoller = Parent as HotdogRoller;
        var matGroup = IndicatorLight.BothOff;
        var lSwitch = hotdogRoller.Interactables["L_Switch"] as Switch;
        var rSwitch = hotdogRoller.Interactables["R_Switch"] as Switch;

        TogglePower = !TogglePower;

        if (lSwitch.TogglePower && rSwitch.TogglePower)
        {
            matGroup = IndicatorLight.BothOn;
        } else if (lSwitch.TogglePower)
        {
            matGroup = IndicatorLight.BackOn;
        } else if (rSwitch.TogglePower)
        {
            matGroup = IndicatorLight.FrontOn;
        }

        parent.SetAnimParameter(animName, TogglePower);
        parent.SetMaterialGroup((int)matGroup);

        Sound.FromEntity("switch_press_01", parent);

        if (matGroup != IndicatorLight.BothOff)
            Sound.FromEntity("machine_turn_01", parent);
    }
}
