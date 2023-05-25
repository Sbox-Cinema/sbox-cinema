using Sandbox;

namespace Cinema;

public partial class HotdogRoller
{
    public void TryInteraction(string groupName)
    {
        switch (groupName)
        {
            case "hotdog_roller":
                break;

            case "l_handle":
                var leftKnob = Components.Get<LeftKnob>();

                if(leftKnob != null)
                {
                    leftKnob.IncrementPos();
                }
                
                break;

            case "r_handle":
                var rightKnob = Components.Get<RightKnob>();

                if (rightKnob != null)
                {
                    rightKnob.IncrementPos();
                }
                break;


            case "l_switch":
                var leftSwitch = Components.Get<LeftSwitch>();

                if (leftSwitch != null)
                {
                    leftSwitch.TogglePos();
                }
                
                break;

            case "r_switch":
                var rightSwitch = Components.Get<RightSwitch>();

                if (rightSwitch != null)
                {
                    rightSwitch.TogglePos();
                }
                
                break;

            case "roller1":
            case "roller2":
            case "roller3":
            case "roller4":
            case "roller5":
            case "roller6":
            case "roller7":
            case "roller8":
            case "roller9":
            case "roller10":
                break;

        }
    }

    [GameEvent.Tick]
    void UpdateInteractions()
    {
        if (Game.LocalClient.Pawn is Player player)
        {
            TraceResult tr = Trace.Ray(player.AimRay, 2000)
                            .EntitiesOnly()
                            .Ignore(player)
                            .WithoutTags("cookable")
                            .Run();

            if (tr.Hit)
            {
                if (tr.Body is PhysicsBody body)
                {
                    if (Input.Pressed("use"))
                    {
                        TryInteraction(body.GroupName);
                    }
                }
            }
        }
    }
}
