using Cinema;
using System;

namespace Sandbox.util;

public class CanTriggerResults
{
    public CanTriggerResults(bool hit, float dist, BaseInteractable interactable)
    {
        Hit = hit;
        Distance = dist;
        Interactable = interactable;
    }

    public bool Hit;
    public float Distance;
    public BaseInteractable Interactable;
}

public partial class BaseInteractable : BaseNetworkable
{
    [Net]
    public Entity Parent { get; set; }
    [Net]
    public Vector3 Mins { get; set; }
    [Net]
    public Vector3 Maxs { get; set; }
    public float MaxDistance { get; set; } = 60f;

    public BaseInteractable()
    {
    }

    public virtual void Trigger(Cinema.Player ply = null)
    {
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

            var offset = box.OriginOffset;
            var halfDimensions = box.Dimensions;
            halfDimensions.x = halfDimensions.x * .5f;
            halfDimensions.y = halfDimensions.y * .5f;
            halfDimensions.z = halfDimensions.z * .5f;

            var bbox = new BBox(offset - halfDimensions, offset + halfDimensions);

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
        var hit = bounds.Trace(ray, MaxDistance, out float dist);
        var triggerResults = new CanTriggerResults(hit, dist, this);

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

    public virtual void Tick()
    {

    }
}
