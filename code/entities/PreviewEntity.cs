using Sandbox;
using Sandbox.Engine.Utility.RayTrace;

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

    internal bool UpdateFromTrace(MeshTraceRequest.Result tr)
    {
        if (!IsTraceValid(tr))
        {
            return false;
        }

        if(!tr.SceneObject.IsValid())
        {
            return false;
        }

        if (RelativeToNormal)
        {
            var direction = (tr.EndPosition - tr.StartPosition).Normal;
            Rotation = Rotation.LookAt(tr.HitNormal, direction) * RotationOffset;
            Position = (tr.StartPosition + direction * tr.Distance) + Rotation * PositionOffset;
        }
        else
        {
            var direction = (tr.EndPosition - tr.StartPosition).Normal;
            Rotation = Rotation.Identity * RotationOffset;
            Position = (tr.StartPosition + direction * tr.Distance) + PositionOffset;
        }

        return true;
    }

    protected virtual bool IsTraceValid(TraceResult tr) => tr.Hit;
    protected virtual bool IsTraceValid(MeshTraceRequest.Result tr) => tr.Hit;
}
