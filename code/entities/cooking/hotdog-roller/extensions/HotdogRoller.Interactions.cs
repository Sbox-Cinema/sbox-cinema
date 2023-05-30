namespace Cinema;

public partial class HotdogRoller
{
    /// <summary>
    ///
    /// </summary>
    public void TryInteraction(string groupName)
    {
        switch (groupName)
        {
            case "hotdog_roller":
                break;

            case "l_handle":
                LeftKnob.IncrementPos();

                break;

            case "r_handle":
                RightKnob.IncrementPos();
                
                break;

            case "l_switch":
                LeftSwitch.TogglePos();

                UpdatePowerState();
                
                break;

            case "r_switch":
                RightSwitch.TogglePos();

                UpdatePowerState();
                
                break;

            case "roller1":
                Roller.AddHotdog(1);

                break;

            case "roller2":
                Roller.AddHotdog(2);

                break;

            case "roller3":

                Roller.AddHotdog(3);
                break;

            case "roller4":
                Roller.AddHotdog(4);

                break;

            case "roller5": 
                Roller.AddHotdog(5);
                
                break;

            case "roller6":
                Roller.AddHotdog(6);
                
                break;

            case "roller7":
                Roller.AddHotdog(7);
                
                break;

            case "roller8":
                Roller.AddHotdog(8);
                
                break;

            case "roller9":
                Roller.AddHotdog(9);
                
                break;

            case "roller10":
                Roller.AddHotdog(10);

                break;

        }
    }
}
