using Cinema;
using System.Collections.Generic;

namespace Sandbox.util;
public class Slot
{
    public Entity Entity { get; set; }
    public string Attachment { get; set; }
    public int Index { get; set; }

    public int MaxDistanceTarget = 8; // This is the max distance to target the specific target
    public Slot(int index, string attachment)
    {
        Index = index;
        Attachment = attachment;
    }

    public void SetItem(Entity entity)
    {
        Entity = entity;
    }
    public void Clear()
    {
        Entity.Delete();
    }
    public bool IsEmpty()
    {
        return !Entity.IsValid();
    }

    public bool HasItem()
    {
        return Entity.IsValid();
    }
}

public class CanTriggerResults
{
    public CanTriggerResults(bool hit, float distance, BaseInteractable interactable)
    {
        Hit = hit;
        Distance = distance;
        Interactable = interactable;
    }

    public bool Hit;
    public float Distance;
    public BaseInteractable Interactable;
}

public partial class BaseInteractable : BaseNetworkable
{
    [Net] public string Name { get; set; }
    [Net] public Entity Parent { get; set; }
    [Net] public Vector3 Mins { get; set; }
    [Net] public Vector3 Maxs { get; set; }
    [Net] public string Attachment { get; set; }
    public float MaxDistance { get; set; } = 60f;

    public BaseInteractable()
    {
    }

    public virtual void Trigger(Cinema.Player player = null)
    {
    }

    public virtual void Simulate()
    {
    }

    public Transform GetParentTransform(string attachment)
    {
        if ((Parent as ModelEntity).GetAttachment(attachment) is Transform transform)
        {
            return transform;
        }

        return new Transform();
    }

    /// <summary>
    /// Sets the bounds based on the GameData "interact_box", pass the name defined in ModelDoc.
    /// If it doesnt find anything it wont set anything.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public BaseInteractable SetBoundsFromInteractionBox(string name)
    {
        var interactionBoxes = (Parent as ModelEntity).Model.GetData<ModelInteractionBox[]>();

        foreach (var box in interactionBoxes)
        {
            if (box.Name != name)
                continue;

            Name = box.Name;
            Attachment = box.Attachment;
            
            var offset = box.OriginOffset;
            var halfDimensions = box.Dimensions * 0.5f;

            var parent = Parent as ModelEntity;

            var attachment = parent.GetAttachment(Attachment, false);

            var bbox = new BBox( (offset + attachment.Value.Position) - halfDimensions , (offset + attachment.Value.Position) + halfDimensions );

            Mins = bbox.Mins;
            Maxs = bbox.Maxs;

            break;
        }

        return this;
    }

    /// <summary>
    /// Sets the parent of the interactable, has to be an entity.
    /// </summary>
    /// <param name="parent"></param>
    /// <returns></returns>
    public BaseInteractable SetParent(Entity parent)
    {
        Parent = parent;

        return this;
    }

    /// <summary>
    /// This one will check if you can trigger, returns a struct with information about the trace.
    /// </summary>
    /// <param name="ray"></param>
    /// <returns></returns>
    public CanTriggerResults CanRayTrigger(Ray ray)
    {
        var mins = Parent.Transform.PointToWorld(Mins);
        var maxs = Parent.Transform.PointToWorld(Maxs);
        var bounds = new BBox(mins, maxs); // Would be nice if FP returned the HitPosition here.
        var hit = bounds.Trace(ray, MaxDistance, out float distance);
        var triggerResults = new CanTriggerResults(hit, distance, this);

        return triggerResults;
    }

    /// <summary>
    /// This will check if you can trigger, if it can it will actually trigger.
    /// </summary>
    /// <param name="ray"></param>
    /// <returns></returns>
    public bool TryTrigger(Ray ray)
    {
        var canTrigger = CanRayTrigger(ray);

        if (canTrigger.Hit)
        {
            Trigger();
        }

        return canTrigger.Hit;
    }
}
