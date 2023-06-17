namespace Sandbox.util;

public struct CanTriggerResults
{
    public CanTriggerResults(bool hit, float dist)
    {
        Hit = hit;
        Distance = dist;
    }

    public bool Hit;
    public float Distance;
}

public partial class BaseInteractable : BaseNetworkable
{
    [Net]
    public Entity Parent { get; set; }
    [Net]
    public Vector3 Mins { get; set; }
    [Net]
    public Vector3 Maxs { get; set; }
    public float MaxDistance { get; set; } = 90f;

    public BaseInteractable()
    {
    }

    public virtual void Trigger()
    {
    }

    public CanTriggerResults CanTrigger(Ray ray)
    {
        var mins = Parent.Transform.PointToWorld(Mins);
        var maxs = Parent.Transform.PointToWorld(Maxs);
        var bounds = new BBox(mins, maxs);
        var hit = bounds.Trace(ray, MaxDistance, out float dist);

        return new CanTriggerResults(hit, dist);
    }

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
