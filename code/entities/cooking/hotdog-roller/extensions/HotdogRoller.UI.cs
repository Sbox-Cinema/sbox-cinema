using Sandbox;

namespace Cinema;

public partial class HotdogRoller
{
    public string UseText { get; set; }

    /// <summary>
    /// Sets up the UI when the machine is interacted with
    /// </summary>
    private void SetupUI()
    {
        
    }

    /// <summary>
    /// Called when the player intersects with an interaction volume
    /// </summary>
    private void OnInteractionVolumeHover(string groupName)
    {
        switch(groupName)
        {
            case "hotdog_roller":
                // Ignore
                break;
            case "l_handle":
                UseText = "Change Back Roller Temperature";
                break;

            case "r_handle":
                UseText = "Change Front Roller Temperature";
                break;
            case "l_switch":
                UseText = "Toggle Back Roller Power";
                break;
            case "r_switch":
                UseText = "Toggle Front Roller Power";
                break;
            case "roller1":
                UseText = "Add Front Roller Hotdog";
                break;
            case "roller6":
                UseText = "Add Back Roller Hotdog";
                break;
        }
    }

    /// <summary>
    /// Raycast to see if player is in contact with any interactables
    /// </summary>
    [GameEvent.Tick]
    public void Update()
    {
        FindInteractable();
    }

    private void FindInteractable()
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
                        DrawVolume(body, false);
                    }
                    else if (body.GroupName != "")
                    {
                        DrawVolume(body, true);
                        DrawCursor(tr.EndPosition);

                        OnInteractionVolumeHover(body.GroupName);
                    }
                }

                if (tr.Body.GroupName == "")
                {
                    UseText = "For Hotdog Roller Info";
                }
            }

        }
    }

    /// <summary>
    /// Draws interaction volume
    /// </summary>
    private void DrawVolume(PhysicsBody body, bool active)
    {   
        BBox bbox = body.GetBounds();

        Color inactiveColor = Color.Gray;
        Color activeColor = Color.White;

        DebugOverlay.Box(bbox.Mins, bbox.Maxs, !active ? inactiveColor : activeColor);
    }

    /// <summary>
    /// Draws interaction cursor
    /// </summary>
    private void DrawCursor(Vector3 pos)
    {
        DebugOverlay.Sphere(pos, 0.25f, Color.White);
    }
}
