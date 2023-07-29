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
    /// This will add or take the cup you are pressing closest to, if nothing is close to it, it will add a cup to the nearest possible slot.
    /// </summary>
    /// <param name="player"></param>
    public override void Trigger(Player player)
    {
        var slot = GetClosestSlot(player);

        if (slot.HasContents())
        {
            TakeCup(slot, player);

            return;
        }

        if (slot.IsEmpty())
        {
            AddCup(slot);
        }
    }

    /// <summary>
    /// Gets the slot the player was to add/take an item from
    /// </summary>
    /// <param name="player"></param>
    /// <returns>The closest slot to the player's aim ray</returns>
    private Slot GetClosestSlot(Player player)
    {
        var ray = player.AimRay;
        var tr = Trace.Ray(ray, MaxDistance)
                   .WithoutTags("player")
                   .DynamicOnly()
                   .Run();

        return Slots.OrderBy(x => tr.HitPosition.Distance(GetParentTransform(x.Attachment).Position)).FirstOrDefault();
    }

    /// <summary>
    /// Adds a cup to the platform if the dispenser isn't already running
    /// </summary>
    /// <param name="slot"></param>
    private void AddCup(Slot slot)
    {
        var dispenser = (Parent as SodaFountain).FindInteractable($"tap_{slot.Index + 1}") as Dispenser;

        // Don't add cup if the dispenser is already dispensing
        if (dispenser.IsDispensing) return;

        // Play sound for cup placement
        Sound.FromEntity("cup_place", Parent);

        var cup = CreateCup(slot, dispenser);

        // Add entity to this slot
        slot.SetContents(cup);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="slot"></param>
    /// <param name="player"></param>
    private void TakeCup(Slot slot, Player player)
    {
        var dispenser = (Parent as SodaFountain).FindInteractable($"tap_{slot.Index + 1}") as Dispenser;
        var cup = slot.Entity as FillableCup;
 
        // Dont take cup if dispenser hasn't finished dispensing
        if (!cup.IsAssembled || dispenser.IsDispensing) return;

        // Add cup to player's inventory
        player.PickupItem(InventorySystem.CreateItem(cup.ItemId));

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
            ItemId = SodaFountain.GetCupItemIdBySodaType(dispenser.SodaType),
            Dispenser = dispenser
        };
    
        cup.Initialize();

        return cup;
    }
}
