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
        var slot = GetClosestSlot(player);

        if (slot.IsEmpty())
        {
            AddCup(slot);
        }

        if (slot.HasItem())
        {
            TakeCup(slot, player);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="player"></param>
    /// <returns> </returns>
    private Slot GetClosestSlot(Player player)
    {
        var ray = player.AimRay;
        var tr = Trace.Ray(ray, MaxDistance)
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

        return slotsByDistance
                .OrderBy(x => x.Value)
                .Select(x => x.Key)
                .FirstOrDefault();
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

        var cup = CreateCup(slot, dispenser);

        // Add entity to this slot
        slot.SetItem(cup);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="slot"></param>
    /// <param name="player"></param>
    /// <returns> </returns>
    private void TakeCup(Slot slot, Player player)
    {
        if ((Parent as SodaFountain).Interactables[$"Dispenser{slot.Index + 1}"] is not Dispenser dispenser) return;

        if (slot.Entity is not FillableCup cup || !cup.Assembled() || dispenser.IsDispensing) return;

        // Add cup to player's inventory
        player.PickupItem(InventorySystem.CreateItem(cup.ItemId()));

        // Remove entity from this slot
        slot.Clear();   
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="slot"></param>
    /// <param name="dispenser"></param>
    /// <returns></returns>
    private FillableCup CreateCup(Slot slot, Dispenser dispenser)
    {
        var cup = new FillableCup()
        {
            Transform = GetParentTransform(slot.Attachment),
            Parent = Parent,
            CupItemUniqueId = SodaFountain.GetCupItemIdBySodaType(dispenser.SodaType),
            Dispenser = dispenser
        };
    
        cup.Initialize();

        return cup;
    }
}
