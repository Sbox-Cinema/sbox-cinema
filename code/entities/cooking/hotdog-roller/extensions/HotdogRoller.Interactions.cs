namespace Cinema;

public partial class HotdogRoller
{
    /// <summary>
    /// Try to interact with an interact volume
    /// </summary>
    public void TryInteraction(string groupName)
    {
        switch (groupName)
        {
            case "hotdog_roller":
                break;

            case "l_handle":
                Knobs.IncrementBackRollerKnobPos();

                break;

            case "r_handle":
                Knobs.IncrementFrontRollerKnobPos();

                break;

            case "l_switch":
                Switches.ToggleBackRollerPower();
             
                break;

            case "r_switch":
                Switches.ToggleFrontRollerPower();
                
                break;

            case "roller1":
                Rollers.AddFrontRollerHotdog();

                break;

            case "roller6":
                Rollers.AddBackRollerHotdog();
                
                break;
        }
    }
}
