using Sandbox;

namespace Cinema.Cookable;

public partial class Hotdog
{
    [BindComponent] public Rotator Rotator { get; }
    public override void Spawn()
    {
        base.Spawn();

        SetupModel();

        Components.Create<Rotator>();
    }
    public override void ClientSpawn()
    {
        base.ClientSpawn();
    }

    /// <summary>
    /// Sets up the model when spawned by the server
    /// </summary>
    private void SetupModel()
    {
        Transmit = TransmitType.Always;

        SetModel("models/hotdog/hotdog_cookable.vmdl");

        SetupPhysicsFromModel(PhysicsMotionType.Keyframed);

        Tags.Add("cookable");
    }
}
