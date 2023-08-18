using Sandbox;

public partial class GooglyEyeEntity : ModelEntity
{
    public override void Spawn()
    {
        base.Spawn();

        SetupPhysicsFromModel(PhysicsMotionType.Keyframed);
    }
}
