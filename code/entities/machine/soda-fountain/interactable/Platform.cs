using System.Collections.Generic;
using System.Linq;
using Sandbox;
using Sandbox.util;
using Conna.Inventory;

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
        var dispenserKey = $"Dispenser{slot.Index + 1}";
        var dispenser = (Parent as SodaFountain).Interactables[dispenserKey] as Dispenser;

        // Don't add cup if the dispenser is already dispensing
        if (dispenser.IsDispensing) return;

        var cup = new CupFillable(dispenser)
        {
            Transform = GetParentTransform(slot.Attachment),
            Parent = Parent
        };

        cup.SetCupColor(dispenser.SodaType);

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
                AddCup(slot);

                break;
            }

            if (distance < MaxDistanceTarget)
            { 
                if(slot.Entity is CupFillable cup && cup.CanPickup()) 
                {
                    PickupCup(slot, player);   
                }

                break;
            }
        }
    }

    private void PickupCup(Slot slot, Player player)
    {
        var cupColor = (slot.Entity as CupFillable).GetMaterialGroup();

        switch (cupColor)
        {
            case (int)CupFillable.MaterialGroup.Red:
                player.PickupItem(InventorySystem.CreateItem("soda-conk"));
                break;
            case (int)CupFillable.MaterialGroup.Blue:
                player.PickupItem(InventorySystem.CreateItem("soda-mionpisz"));
                break;
            case (int)CupFillable.MaterialGroup.Green:
                player.PickupItem(InventorySystem.CreateItem("soda-spooge"));
                break;
            default:
                player.PickupItem(InventorySystem.CreateItem(CupItemUniqueId));
                break;
        }

        // Remove entity from this slot
        slot.Entity.Delete();

        // Remove cup from dispenser parent
        var dispenserKey = $"Dispenser{slot.Index + 1}";

        var dispenser = (Parent as SodaFountain).Interactables[dispenserKey] as Dispenser;
        dispenser.Cup?.Delete();
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
