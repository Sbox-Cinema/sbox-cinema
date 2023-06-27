using Editor;
using Sandbox;
using Sandbox.util;
using System.Collections.Generic;

namespace Cinema;

[Library("ent_soda_fountain"), HammerEntity]
[Title("Soda Fountain"), Category("Gameplay"), Icon("coffee_maker")]
[EditorModel("models/sodafountain/sodafountain_01.vmdl")]
public partial class SodaFountain : AnimatedEntity, ICinemaUse
{
    [Net] public IDictionary<string, BaseInteractable> Interactables { get; set; }

    /// <summary>
    /// Set up the model when spawned by the server
    /// Setup model
    /// Setup interactions
    /// </summary>
    public override void Spawn()
    {
        base.Spawn();

        Transmit = TransmitType.Always;

        SetModel("models/sodafountain/sodafountain_01.vmdl");

        SetupPhysicsFromModel(PhysicsMotionType.Keyframed);

        AddInteractables();

        Tags.Add("interactable");
    }

    public void HandleUse(Entity player)
    {

        foreach (var interactableData in Interactables)
        {
            var interactable = interactableData.Value;

            interactable.Trigger(player as Player);
        }

        /*
        BaseInteractable found = null;
        float nearest = 999999;

        foreach (var interactableData in Interactables)
        {

            var interactable = interactableData.Value;
            var result = interactable.CanRayTrigger(player.AimRay);

            if (result.Hit && result.Distance < interactable.MaxDistance && result.Distance < nearest)
            {
                nearest = result.Distance;
                found = interactable;
            }
        }

        if (found != null)
            found.Trigger(player as Player);
        */

    }

    public void AddInteractables()
    {
        Interactables.Add("Dispenser1", new Dispenser("", "D1")
        {
            Parent = this,
            Mins = new Vector3(242, 465.0f, 36.0f),
            Maxs = new Vector3(266, 471.0f, 38.0f)
        });

        Interactables.Add("Dispenser2", new Dispenser("", "D2")
        {
            Parent = this,
            Mins = new Vector3(242, 465.0f, 36.0f),
            Maxs = new Vector3(266, 471.0f, 38.0f)
        });

        Interactables.Add("Dispenser3", new Dispenser("", "D3")

        {
            Parent = this,
            Mins = new Vector3(242, 465.0f, 36.0f),
            Maxs = new Vector3(266, 471.0f, 38.0f)
        });

        Interactables.Add("Platform", new Platform()
        {
            Parent = this,
            Mins = new Vector3(242, 465.0f, 36.0f),
            Maxs = new Vector3(266, 471.0f, 38.0f)
        });
    }

    
}
