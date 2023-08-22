using Sandbox;

public partial class GooglyEyeEntity : ModelEntity
{
    public override void Spawn()
    {
        base.Spawn();

        SetModel("models/googly_eyes/googly_eyes_01.vmdl");

        SetupPhysicsFromModel(PhysicsMotionType.Keyframed);
    }
}
