using Sandbox;

namespace Cinema;

public partial class PreviewEntity : ModelEntity
{
    public bool RelativeToNormal { get; set; } = true;

    protected virtual bool IsTraceValid(TraceResult tr) => tr.Hit;

    internal bool UpdateFromTrace(TraceResult tr)
    {
        if (!IsTraceValid(tr)) return false;
        
        if(RelativeToNormal)
        {
            Rotation = Rotation.LookAt(tr.Normal, tr.Direction);
            Position = tr.EndPosition;
        }
        else
        {
            Rotation = Rotation.Identity;
            Position = tr.EndPosition;
        }

        return true;
    }
}
