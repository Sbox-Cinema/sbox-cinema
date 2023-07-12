using System.Collections.Generic;
using System.Linq;
using Sandbox;
using Sandbox.util;
using Conna.Inventory;

namespace Cinema;

public class Platform : BaseInteractable
{
    static private int NumSlots = 3;

    private Slot[] Slots = new Slot[NumSlots];
    
    public Platform() 
    {
        for (int i = 0; i < NumSlots; i++)
        {
            Slots[i] = new Slot(i, $"lever{i + 1}_cup");
        }
    }

    /// <summary>
    /// This will add or take the cup you are pressing closest to, if nothing is close it will just add a cup to the nearest possible slot.
    /// </summary>
    /// <param name="player"></param>
    public override void Trigger(Player player)
    {
        var ray = player.AimRay;
        var tr = Trace.Ray(ray.Position, ray.Position + (ray.Forward.Normal * MaxDistance))
        .WithoutTags("player")
        .DynamicOnly()
        .Run();

        IDictionary<Slot, float> slotsByDistance = new Dictionary<Slot, float>();

        for (int i = 0; i < NumSlots; i++)
        {
            var slot = Slots[i];
            var distance = tr.HitPosition.Distance(GetParentTransform(slot.Attachment).Position);

            slotsByDistance.Add(slot, distance);
        }

        foreach (var (slot, distance) in slotsByDistance.OrderBy(x => x.Value))
        {
            if (slot.IsEmpty())
            {
                AddCup(slot);

                break;
            }

            if (distance < slot.MaxDistanceTarget)
            { 
                PickupCup(slot, player);   
                
                break;
            }
        }
    }

    /// <summary>
    /// Adds a cup to the platform if the dispenser isn't already running
    /// </summary>
    /// <param name="slot"></param>
    private void AddCup(Slot slot)
    {
        var dispenser = (Parent as SodaFountain).Interactables[$"Dispenser{slot.Index + 1}"] as Dispenser;

        // Don't add cup if the dispenser is already dispensing
        if (dispenser.IsDispensing) return;

        // Play sound for cup placement
        Sound.FromEntity("cup_place", Parent);

        var cup = new CupFillable(dispenser)
        {
            Transform = GetParentTransform(slot.Attachment),
            Parent = Parent
        };

        cup.SetCupColor(dispenser.SodaType);

        slot.Entity = cup;
    }

    private void PickupCup(Slot slot, Player player)
    {
        if (slot.Entity is CupFillable cup && cup.CanPickup())
        {
            var cupColor = cup.GetMaterialGroup();

            switch (cupColor)
            {
                case (int)CupFillable.MaterialGroup.Red:
                    player.PickupItem(InventorySystem.CreateItem(CupFillable.CupItemUniqueIdConk));
                    break;
                case (int)CupFillable.MaterialGroup.Blue:
                    player.PickupItem(InventorySystem.CreateItem(CupFillable.CupItemUniqueIdMionPisz));
                    break;
                case (int)CupFillable.MaterialGroup.Green:
                    player.PickupItem(InventorySystem.CreateItem(CupFillable.CupItemUniqueIdSpooge));
                    break;
                default:
                    player.PickupItem(InventorySystem.CreateItem(CupFillable.CupItemUniqueId));
                    break;
            }

            // Remove entity from this slot
            slot.Entity.Delete();

            // Remove cup from dispenser parent
            var dispenser = (Parent as SodaFountain).Interactables[$"Dispenser{slot.Index + 1}"] as Dispenser;
            dispenser.Cup?.Delete();
        }
    }
}
