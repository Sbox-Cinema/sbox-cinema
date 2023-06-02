namespace Cinema;

public partial class HotdogRoller
{
    /// <summary>
    /// Try interact with an interact volume
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
                Rollers.AddHotdog(1);

                break;

            case "roller2":
                Rollers.AddHotdog(2);

                break;

            case "roller3":
                Rollers.AddHotdog(3);

                break;

            case "roller4":
                Rollers.AddHotdog(4);

                break;

            case "roller5": 
                Rollers.AddHotdog(5);
                
                break;

            case "roller6":
                Rollers.AddHotdog(6);
                
                break;

            case "roller7":
                Rollers.AddHotdog(7);
                
                break;

            case "roller8":
                Rollers.AddHotdog(8);
                
                break;

            case "roller9":
                Rollers.AddHotdog(9);
                
                break;

            case "roller10":
                Rollers.AddHotdog(10);

                break;

        }
    }
}
