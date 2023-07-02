using Conna.Inventory;
using Sandbox;
using Sandbox.util;
using System.Collections.Generic;
using System.Linq;

namespace Cinema;

public class Platform : BaseInteractable
{
    private class Slot
    {
        public CupFillable Entity { get; set; }
        public string Attachment { get; set; }
        public int Index { get; set; }
        public Slot(int index, string attachment)
        {
            Index = index;
            Attachment = attachment;
        }
    }

    static private int NumSlots = 3;

    private Slot[] Slots = new Slot[NumSlots];
    private SodaFountain PlatformParent => Parent as SodaFountain;
    
    private int MaxDistanceTarget = 16; // This is the max distance to target the specific cup
    
    private string CupItemUniqueId = "soda"; // The unique id for the cup item

    public Platform() 
    {
        for (int i = 0; i < NumSlots; i++)
        {
            Slots[i] = new Slot(i, $"lever{i + 1}_cup");
        }
    }
    private void AddCup(Slot slot)
    {
        var dispenser = (Parent as SodaFountain).Interactables[$"Dispenser{slot.Index + 1}"] as Dispenser;

        // Don't add cup if the dispenser is already dispensing
        if (dispenser.IsDispensing) return;
        
        var cup = new CupFillable(dispenser);
        cup.Transform = GetParentTransform(slot.Attachment);
        cup.Parent = Parent;

        // Set slot's entity to cup
        slot.Entity = cup;
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
            if (!slot.Entity.IsValid())
            {
                // Add cup to this slot
                AddCup(slot);

                break;
            }

            if (distance < MaxDistanceTarget)
            { 
                if(slot.Entity is CupFillable cup) {

                    if (cup.CanPickup())
                    {
                        // Remove entity from this slot
                        slot.Entity.Delete();

                        // Pickup cup
                        if ((slot.Entity as CupFillable).GetMaterialGroup() == (int)CupFillable.CupColor.Blue)
                        {
                            var cupCarriable = InventorySystem.CreateItem("soda-blue");
                            player.PickupItem(cupCarriable);
                        } 
                        else if((slot.Entity as CupFillable).GetMaterialGroup() == (int)CupFillable.CupColor.Green)
                        {
                            var cupCarriable = InventorySystem.CreateItem("soda-green");
                            player.PickupItem(cupCarriable);
                        }
                        else if ((slot.Entity as CupFillable).GetMaterialGroup() == (int)CupFillable.CupColor.Red)
                        {
                            var cupCarriable = InventorySystem.CreateItem("soda-red");
                            player.PickupItem(cupCarriable);
                        }
                        else if ((slot.Entity as CupFillable).GetMaterialGroup() == (int)CupFillable.CupColor.Black)
                        {
                            var cupCarriable = InventorySystem.CreateItem("soda-black");
                            player.PickupItem(cupCarriable);
                        }
                        else
                        {
                            var cupCarriable = InventorySystem.CreateItem(CupItemUniqueId);
                            player.PickupItem(cupCarriable);
                        }
                        
                        // Remove cup from dispenser parent
                        var dispenser = (Parent as SodaFountain).Interactables[$"Dispenser{slot.Index + 1}"] as Dispenser;
                        dispenser.Cup?.Delete();
                    }
                }

                break;
            }
        }
    }
    private Transform GetParentTransform(string attachment)
    {
        if (PlatformParent.GetAttachment(attachment) is Transform transform)
        {
            return transform;
        }

        return new Transform();
    }
}
