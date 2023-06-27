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
            Slots[i] = new Slot(i, $"S{i + 1}");
        }
    }

    private CupFillable AddCup(Slot slot)
    {
        var sodaFountain = Parent as SodaFountain;

        var cup = new CupFillable(sodaFountain.Interactables[$"Dispenser{slot.Index+1}"] as Dispenser);

        cup.Transform = GetParentTransform(slot.Attachment);
        
        //Adjust cup to fit underneath dispenser
        cup.LocalScale = 0.45f;
        cup.Position -= Vector3.Up * 1.35f;
        
        cup.Parent = Parent;

        slot.Entity = cup;

        switch(slot.Index + 1)
        {
            case 1:
                Particles soda1 = Particles.Create("particles/soda_fountain/walker/sodafill2_f_conk.vpcf", Parent);
                soda1.SetEntityAttachment(0, Parent, $"D{slot.Index + 1}");
                break; 
            case 2:
                Particles soda2 = Particles.Create("particles/soda_fountain/walker/sodafill2_f_mionpisz.vpcf", Parent);
                soda2.SetEntityAttachment(0, Parent, $"D{slot.Index + 1}");
                break;
            case 3:
                Particles soda3 = Particles.Create("particles/soda_fountain/walker/sodafill2_f_spooge.vpcf", Parent);
                soda3.SetEntityAttachment(0, Parent, $"D{slot.Index + 1}");
                break;
        }
        

        return cup;
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
                slot.Entity.Delete();
             
                var cupCarriable = InventorySystem.CreateItem(CupItemUniqueId);

                player.PickupItem(cupCarriable);

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
