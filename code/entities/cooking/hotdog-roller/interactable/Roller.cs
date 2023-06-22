using Conna.Inventory;
using Sandbox;
using Sandbox.util;
using System.Collections.Generic;

namespace Cinema.HotdogRoller;

public class Roller : BaseInteractable
{
    private struct Slot
    {
        public HotdogCookable Entity { get; set; }
        public string Attachment { get; set; }

        public Slot(string attachment)
        {
            Attachment = attachment;
        }
    }

    private struct SlotDistance
    {
        public Slot Slot { get; set; }
        public float Distance { get; set; }
        public int Index { get; set; }
        public Transform SlotTransform { get; set; }

        public SlotDistance(Slot slot, float dist, int index, Transform t)
        {
            Slot = slot;
            Distance = dist;
            Index = index;
            SlotTransform = t;
        }
    }

    public Knob Knob { get; set; }
    public Switch Switch { get; set; }
    private Slot[] slots = new Slot[10];

    public Roller() // For the compiler...
    {
    }

    public Roller(Switch parentSwitch, Knob knob, bool front = false)
    {
        Switch = parentSwitch;
        Knob = knob;

        for (int i = 0; i < 10; i++)
        {
            slots[i] = new Slot($"S{i + 1}{(front ? 'F' : 'B')}");
        }
    }

    private HotdogCookable AddHotdog(Roller parent, Transform t)
    {
        var hotdog = new HotdogCookable(parent);
        var reverse = Game.Random.Int(1) == 1;

        hotdog.Transform = t;
        hotdog.LocalRotation = hotdog.LocalRotation.RotateAroundAxis(Vector3.Forward, Game.Random.Float(180));
        hotdog.LocalRotation = hotdog.LocalRotation.RotateAroundAxis(Vector3.Up, reverse ? 180 : 0);
        hotdog.Parent = Parent;

        return hotdog;
    }

    /// <summary>
    /// This will add or take the hotdog you are pressing closest to, if nothing is close it will just add a hotdog to the nearest possible slot.
    /// </summary>
    /// <param name="ply"></param>
    public override void Trigger(Player ply)
    {
        List<SlotDistance> slotsIndexed = new List<SlotDistance>();
        var parent = Parent as HotdogRoller;

        for (int i = 0; i < 10; i++)
        {
            var slotData = slots[i];

            if (parent.GetAttachment(slotData.Attachment) is Transform t)
            {
                var dist = LastTriggerResults.HitPosition.Distance(t.Position);

                slotsIndexed.Add(new SlotDistance(slotData, dist, i, t));
            }
        }

        slotsIndexed.Sort((x, y) => { return x.Distance.CompareTo(y.Distance); });

        foreach(var slotCompared in slotsIndexed)
        {
            var slotData = slotCompared.Slot;
            if (slotData.Entity is null || !slotData.Entity.IsValid)
            {
                var hotdog = AddHotdog(this, slotCompared.SlotTransform);

                slotData.Entity = hotdog;
                slots[slotCompared.Index] = slotData;

                break;
            }
            else if (slotData.Entity.GetMaterialGroup() > 1)
            {
                if (slotCompared.Distance < 5)
                {
                    slotData.Entity.Delete();

                    var hotdogCarriable = InventorySystem.CreateItem("hotdog_cooked");

                    ply.PickupItem(hotdogCarriable);

                    break;
                }

            }
        }
    }
}
