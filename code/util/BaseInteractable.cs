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
