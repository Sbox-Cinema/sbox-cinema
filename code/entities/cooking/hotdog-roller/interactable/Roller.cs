using Conna.Inventory;
using Sandbox;
using Sandbox.util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Cinema.HotdogRoller;

public class Roller : BaseInteractable
{
    private struct Slot
    {
        public HotdogCookable Entity { get; set; }
        public string Attachment { get; set; }
        public int Index { get; set; }

        public Slot(int index, string attachment)
        {
            Index = index;
            Attachment = attachment;
        }
    }

    public Knob Knob { get; set; }
    public Switch Switch { get; set; }
    private Slot[] Slots = new Slot[10];
    private HotdogRoller RollerParent => Parent as HotdogRoller;

    public Roller() // For the compiler...
    {
    }

    public Roller(Switch parentSwitch, Knob knob, bool front = false)
    {
        Switch = parentSwitch;
        Knob = knob;

        for (int i = 0; i < 10; i++)
        {
            Slots[i] = new Slot(i, $"S{i + 1}{(front ? 'F' : 'B')}");
        }
    }

    private Transform GetParentTransform(string attachment)
    {
        if (RollerParent.GetAttachment(attachment) is Transform transform)
        {
            return transform;
        }

        return new Transform();
    }

    private HotdogCookable AddHotdog(Roller parent, Slot slot)
    {
        var hotdog = new HotdogCookable(parent);
        var reverse = Game.Random.Int(1) == 1;

        hotdog.Transform = GetParentTransform(slot.Attachment);
        hotdog.LocalRotation = hotdog.LocalRotation.RotateAroundAxis(Vector3.Forward, Game.Random.Float(180));
        hotdog.LocalRotation = hotdog.LocalRotation.RotateAroundAxis(Vector3.Up, reverse ? 180 : 0);
        hotdog.Parent = Parent;

        slot.Entity = hotdog;
        Slots[slot.Index] = slot;

        return hotdog;
    }

    /// <summary>
    /// This will add or take the hotdog you are pressing closest to, if nothing is close it will just add a hotdog to the nearest possible slot.
    /// </summary>
    /// <param name="ply"></param>
    public override void Trigger(Player ply)
    {
        var ray = ply.AimRay;
        var tr = Trace.Ray(ray.Position, ray.Position + (ray.Forward.Normal * MaxDistance))
        .WithoutTags("player")
        .EntitiesOnly()
        .Run();

        IDictionary<Slot, float> slotsByDistance = new Dictionary<Slot, float>();

        for (int i = 0; i < 10; i++)
        {
            var slot = Slots[i];
            var dist = tr.HitPosition.Distance(GetParentTransform(slot.Attachment).Position);

            slotsByDistance.Add(slot, dist);
        }

        slotsByDistance.ToList().Sort((x, y) => { return x.Value.CompareTo(y.Value); }); ;

        foreach(var slotDistance in slotsByDistance)
        {
            var slot = slotDistance.Key;
            var distance = slotDistance.Value;

            if (slot.Entity is null || !slot.Entity.IsValid)
            {
                AddHotdog(this, slot);

                break;
            }
            else if (slot.Entity.GetMaterialGroup() > 1 && distance < 5)
            {
                slot.Entity.Delete();

                var hotdogCarriable = InventorySystem.CreateItem("hotdog_cooked");

                ply.PickupItem(hotdogCarriable);

                break;
            }
        }
    }
}
