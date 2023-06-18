namespace Sandbox.util;

public class CanTriggerResults
{
    public CanTriggerResults(bool hit, float dist, BaseInteractable interactable, Vector3 hitpos)
    {
        Hit = hit;
        Distance = dist;
        Interactable = interactable;
        HitPosition= hitpos;
    }

    public bool Hit;
    public float Distance;
    public BaseInteractable Interactable;
    public Vector3 HitPosition;
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
    public CanTriggerResults LastTriggerResults {get; set; }

    public BaseInteractable()
    {
    }

    public virtual void Trigger(Cinema.Player ply = null)
    {
    }

    /// <summary>
    /// This one will check if you can trigger.
    /// </summary>
    /// <param name="ray"></param>
    /// <returns></returns>
    public CanTriggerResults CanTrigger(Ray ray)
    {
        var tr = Trace.Ray(ray.Position, ray.Position + (ray.Forward.Normal * MaxDistance))
            .WithoutTags("player")
            .EntitiesOnly()
            .Run();

        var mins = Parent.Transform.PointToWorld(Mins);
        var maxs = Parent.Transform.PointToWorld(Maxs);
        var bounds = new BBox(mins, maxs); // Would be nice if FP returned the HitPosition here.
        var hit = bounds.Trace(ray, MaxDistance, out float dist);
        var trigger_results = new CanTriggerResults(hit, dist, this, tr.HitPosition);

        LastTriggerResults = trigger_results;

        return trigger_results;
    }

    /// <summary>
    /// This will check if you can trigger, if it can it will actually trigger.
    /// </summary>
    /// <param name="ray"></param>
    /// <returns></returns>
    public bool TryTrigger(Ray ray)
    {
        var canTrigger = CanTrigger(ray);

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
