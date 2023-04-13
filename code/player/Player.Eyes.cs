using Sandbox;
using System.ComponentModel;

namespace Cinema;

public partial class Player
{
    [Net, Predicted, Browsable(false)]
    public Vector3 EyeLocalPosition { get; set; }

    [Net, Predicted, Browsable(false)]
    public Rotation EyeLocalRotation { get; set; }

    [Browsable(false)]
    public Vector3 EyePosition
    {
        get => Transform.PointToWorld(EyeLocalPosition);
        set => EyeLocalPosition = Transform.PointToLocal(value);
    }

    [Browsable(false)]
    public Rotation EyeRotation
    {
        get => Transform.RotationToWorld(EyeLocalRotation);
        set => EyeLocalRotation = Transform.RotationToLocal(value);
    }

    public override Ray AimRay => new Ray(EyePosition, EyeRotation.Forward);
}
