using Sandbox;

namespace Cinema;

public partial class HotdogCookable : AnimatedEntity
{
    [BindComponent] public Rotator Rotator { get; }
    [BindComponent] public Rotator Steam { get; }

    /// <summary>
    /// 
    /// </summary>
    public override void Spawn()
    {
        base.Spawn();

        SetupModel();

        Tags.Add("cookable");
    }
    /// <summary>
    /// 
    /// </summary>
    public override void ClientSpawn()
    {
        base.ClientSpawn();
    }
    private void SetupModel()
    {
        Transmit = TransmitType.Always;

        SetModel("models/hotdog/hotdog_roller.vmdl");

        SetupPhysicsFromModel(PhysicsMotionType.Keyframed);
    }
}
