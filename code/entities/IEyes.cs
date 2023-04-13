namespace Cinema;

public interface IEyes
{
  public Vector3 EyeLocalPosition { get; set; }
  public Rotation EyeLocalRotation { get; set; }

  public Vector3 EyePosition { get; set; }

  public Rotation EyeRotation { get; set; }

  public Ray AimRay { get; }
}
