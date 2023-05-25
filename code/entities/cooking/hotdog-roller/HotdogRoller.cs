using Editor;
using Sandbox;
using System.Collections.Generic;

namespace Cinema;

[Library("ent_hotdog_roller"), HammerEntity]
[Title("Hotdog Roller"), Category("Gameplay"), Icon("microwave")]
[EditorModel("models/hotdogroller/hotdogroller.vmdl")]
public partial class HotdogRoller : AnimatedEntity, ICinemaUse
{
    [Net] public IDictionary<string, Hotdog> Hotdogs {get; set;}

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
                    else if(body.GroupName != "")
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
