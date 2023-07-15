using Sandbox;

namespace Cinema;

public partial class PreviewEntity : ModelEntity
{
    [Net] public bool RelativeToNormal { get; set; } = true;
    [Net] public Rotation RotationOffset { get; set; } = Rotation.Identity;
    [Net] public Vector3 PositionOffset { get; set; } = Vector3.Zero;

    internal bool UpdateFromTrace(TraceResult tr)
    {
        
        if(!IsTraceValid(tr))
        {
            return false;
        }
        

        if(RelativeToNormal)
        {
            Rotation = Rotation.LookAt(tr.Normal, tr.Direction) * RotationOffset;
            Position = tr.EndPosition + Rotation * PositionOffset;
        }
        else
        {
            Rotation = Rotation.Identity * RotationOffset;
            Position = tr.EndPosition + PositionOffset;
        }

        return true;
    }

    protected virtual bool IsTraceValid(TraceResult tr) => tr.Hit;
}
