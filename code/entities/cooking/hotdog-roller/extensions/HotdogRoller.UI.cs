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
            TraceResult tr = Trace.Ray(player.AimRay, 1000)
                            .EntitiesOnly()
                            .WithTag("interactable")
                            .Run();

            if (tr.Hit)
            {
                foreach (var volume in InteractionVolumes)
                {
                    var name = volume.Key;
                    var bounds = volume.Value;

                    if (bounds.Trace(player.AimRay, 1000, out float distance))
                    {
                        DrawVolume(bounds, true);
                        DrawCursor(player.AimRay.Position + (player.AimRay.Forward * distance));

                        OnInteractionVolumeHover(name);
                    }
                    else
                    {
                        DrawVolume(bounds, false);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Draws interaction volume
    /// </summary>
    private void DrawVolume(BBox bounds, bool active)
    {   
        Color inactiveColor = Color.Gray;
        Color activeColor = Color.White;

        DebugOverlay.Box(bounds.Mins, bounds.Maxs, !active ? inactiveColor : activeColor);
    }

    /// <summary>
    /// Draws interaction cursor
    /// </summary>
    private void DrawCursor(Vector3 pos)
    {
        DebugOverlay.Sphere(pos, 0.25f, Color.White);
    }
}
