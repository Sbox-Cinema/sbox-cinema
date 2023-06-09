using Sandbox;
using System.Collections.Generic;
using System.Linq;

namespace Cinema;

public partial class InteractionVolume : BaseNetworkable
{
    public PhysicsBody InteractionBody { get; set; }
    [Net] public Entity Parent { get; set; }
    public string Name { get; set; }
    [Net] public BBox Bounds { get; set; }
    [Net] public string UseText {get; set;}
    public InteractionVolume()
    {

    }
    public InteractionVolume(PhysicsBody body)
    {
        InteractionBody = body;

        Bounds = InteractionBody.GetBounds();
        Name = InteractionBody.GroupName;
        Parent = InteractionBody.GetEntity();
    }
}

public partial class HotdogRollerInteractions : EntityComponent<HotdogRoller>
{
    private float InteractionDistance = 64.0f;
    [Net] private IList<InteractionVolume> InteractionVolumes { get; set; }
    
    protected override void OnActivate()
    {
        base.OnActivate();

        SetupInteractions();
    }

    private void SetupInteractions()
    {
        if (Game.IsClient) return;

        var physBodies = Entity.PhysicsGroup.Bodies.Where((body) => body.GroupName != "");

        foreach (var body in physBodies)
        {
            InteractionVolumes.Add(new InteractionVolume(body));
        }
    }

    /// <summary>
    /// Raycast to see if player is in contact with any interactables
    /// </summary>
    public void Simulate()
    {
        if (Game.IsServer) return;

        FindInteractable();
    }

    public void TryInteractions(Entity user)
    {
        foreach (var volume in InteractionVolumes)
        {
            if (volume.Bounds.Trace(user.AimRay, 1024.0f, out float distance))
            {
                TryInteraction(volume.Name);
            }
        }
    }

    public void TryHotdogRemoval(Entity user)
    {
        foreach (var volume in InteractionVolumes)
        {
            if (volume.Bounds.Trace(user.AimRay, 1024.0f, out float distance))
            {
                if(volume.Name == "roller1")
                {
                    Entity.Rollers.RemoveFrontRollerHotdog();
                }

                if(volume.Name == "roller6")
                {
                    Entity.Rollers.RemoveBackRollerHotdog();
                }
            }
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
                    if (volume.Bounds.Trace(player.AimRay, InteractionDistance, out float distance))
                    {
                        DrawVolume(volume, true);
                        DrawCursor(player.AimRay.Position + (player.AimRay.Forward * distance));
                    }
                    else
                    {
                        DrawVolume(volume, false);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Draws interaction volume
    /// </summary>
    private void DrawVolume(InteractionVolume volume, bool active)
    {
        Color inactiveColor = Color.Gray;
        Color activeColor = Color.White;

        DebugOverlay.Box(volume.Bounds.Mins, volume.Bounds.Maxs, !active ? inactiveColor : activeColor);
    }

    /// <summary>
    /// Draws interaction cursor
    /// </summary>
    private void DrawCursor(Vector3 pos)
    {
        DebugOverlay.Sphere(pos, 0.25f, Color.White);
    }
}
