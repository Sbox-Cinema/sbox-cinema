using Sandbox;
using Sandbox.util;
using System.Collections.Generic;

namespace Cinema;

public partial class SodaFountain : AnimatedEntity, ICinemaUse
{
    [Net] public IDictionary<string, BaseInteractable> Interactables { get; set; }
    public void HandleUse(Entity ply)
    {
        BaseInteractable found = null;
        float nearest = 999999;

        foreach (var interactableData in Interactables)
        {
            var interactable = interactableData.Value;
            var result = interactable.CanRayTrigger(ply.AimRay);

            if (result.Hit && result.Distance < interactable.MaxDistance && result.Distance < nearest)
            {
                nearest = result.Distance;
                found = interactable;
            }
        }

        if (found != null)
            found.Trigger(ply as Player);
    }

    public void AddInteractables()
    {
        Interactables = new Dictionary<string, BaseInteractable>();
    }
}
