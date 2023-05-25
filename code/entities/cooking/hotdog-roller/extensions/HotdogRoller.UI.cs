using Sandbox;

namespace Cinema;

public partial class HotdogRoller
{
    public UI.Tooltip Tooltip { get; set; }
    public string Interaction { get; set; } = "Hotdog Roller";
    public string UseText { get; set; } = $"Press E to use";

    /// <summary>
    /// Sets up the UI when the machine is interacted with
    /// </summary>
    private void SetupUI()
    {
        Tooltip = new UI.Tooltip(this, UseText);
    }

    private void OnInteractionVolumeHover(string groupName)
    {
        switch(groupName)
        {
            case "hotdog_roller":
                Interaction = "Hotdog Roller";
                Tooltip.SetText($"{Interaction}");
                break;

            case "l_handle":
                Interaction = "Back Roller Temperature Knob";
                Tooltip.SetText($"{UseText} {Interaction}");
                break;

            case "r_handle":
                Interaction = "Front Roller Temperature Knob";
                Tooltip.SetText($"{UseText} {Interaction}");
                break;
            case "l_switch":
                Interaction = "Back Roller Power";
                Tooltip.SetText($"{UseText} {Interaction}");
                break;
            case "r_switch":
                Interaction = "Front Roller Power";
                Tooltip.SetText($"{UseText} {Interaction}");
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
                Interaction = "Roller";
                Tooltip.SetText($"{UseText} {Interaction}");
                break;

        }
    }

    /// <summary>
    ///
    /// </summary>
    [GameEvent.Tick]
    public void Update()
    {
        if (Game.LocalPawn is Player player)
        {
            TraceResult tr = Trace.Ray(player.AimRay, 2000)
                            .EntitiesOnly()
                            .Ignore(player)
                            .WithoutTags("cookable")
                            .Run();

            if (tr.Hit)
            {
                foreach (var body in PhysicsGroup.Bodies)
                {
                    if (body.GroupName != "" && body.GroupName != tr.Body.GroupName)
                    {
                        DebugOverlay.Box(body.GetBounds(), Color.White);
                    }
                    else if (body.GroupName != "")
                    {
                        DebugOverlay.Box(body.GetBounds(), Color.Cyan);
                        DebugOverlay.Sphere(tr.EndPosition, 0.5f, Color.Cyan);

                        OnInteractionVolumeHover(body.GroupName);
                    }
                }

                if (tr.Body.GroupName == "")
                {
                    Tooltip.ShouldOpen(false);
                }
                else
                {
                    Tooltip.ShouldOpen(true);
                }
            }
            else
            {
                Tooltip.ShouldOpen(false);
            }
        }
    }
}
