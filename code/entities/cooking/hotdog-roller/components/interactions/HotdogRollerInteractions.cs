using Sandbox;
using System.Collections.Generic;
using System.Linq;

namespace Cinema;

public partial class HotdogRollerInteractions : EntityComponent<HotdogRoller>
{
    
    private float InteractionDistance = 64.0f;
    [Net] private IDictionary<string, BBox> InteractionVolumes { get; set; }
    
    protected override void OnActivate()
    {
        base.OnActivate();

        SetupInteractions();
    }
    public void TryInteractions(Entity user)
    {
        foreach (var volume in InteractionVolumes)
        {
            var name = volume.Key;
            var bounds = volume.Value;

            if (bounds.Trace(user.AimRay, 1024.0f, out float distance))
            {
                TryInteraction(name);
            }
        }
    }
    private void SetupInteractions()
    {
        if (Game.IsClient) return;

        var physBodies = Entity.PhysicsGroup.Bodies.Where((body) => body.GroupName != "");

        foreach (var body in physBodies)
        {
            InteractionVolumes.Add(body.GroupName, body.GetBounds());
        }
    }
    public void TryInteraction(string interactionName)
    {
        switch (interactionName)
        {
            case "l_handle":
                Entity.Knobs.IncrementBackRollerKnobPos();
                break;

            case "r_handle":
                Entity.Knobs.IncrementFrontRollerKnobPos();

                break;

            case "l_switch":
                Entity.Switches.ToggleBackRollerPower();

                break;

            case "r_switch":
                Entity.Switches.ToggleFrontRollerPower();

                break;

            case "roller1":
                Entity.Rollers.AddFrontRollerHotdog();

                break;

            case "roller6":
                Entity.Rollers.AddBackRollerHotdog();

                break;
        }
    }

    /// <summary>
    /// Called when the player intersects with an interaction volume
    /// </summary>
    private void OnInteractionVolumeHover(string groupName)
    {
        switch (groupName)
        {
            case "l_handle":
                Entity.UseText = "Change Back Roller Temperature";
                break;
            case "r_handle":
                Entity.UseText = "Change Front Roller Temperature";
                break;
            case "l_switch":
                Entity.UseText = "Toggle Back Roller Power";
                break;
            case "r_switch":
                Entity.UseText = "Toggle Front Roller Power";
                break;
            case "roller1":
                Entity.UseText = "Add Front Roller Hotdog";
                break;
            case "roller6":
                Entity.UseText = "Add Back Roller Hotdog";
                break;
        }
    }

    /// <summary>
    /// Raycast to see if player is in contact with any interactables
    /// </summary>
    [GameEvent.Tick]
    public void Update()
    {
        if (Game.IsServer) return;

        FindInteractable();
    }

    private void FindInteractable()
    {
        if (Game.LocalPawn is Player player)
        {
            TraceResult tr = Trace.Ray(player.AimRay, InteractionDistance)
                            .EntitiesOnly()
                            .WithTag("interactable")
                            .Run();

            if (tr.Hit && tr.Entity == Entity)
            {
                foreach (var volume in InteractionVolumes)
                {
                    var name = volume.Key;
                    var bounds = volume.Value;

                    if (bounds.Trace(player.AimRay, InteractionDistance, out float distance))
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
