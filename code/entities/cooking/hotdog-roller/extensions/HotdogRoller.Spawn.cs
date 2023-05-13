using Sandbox;

namespace Cinema;

public partial class HotdogRoller
{
    public override void Spawn()
    {
        base.Spawn();

        SetupModel();
    }
    public override void ClientSpawn()
    {
        base.ClientSpawn();

        SetupUI();
    }

    /// <summary>
    /// Sets up the model when spawned by the server
    /// </summary>
    private void SetupModel()
    {
        Transmit = TransmitType.Always;

        SetModel("models/hotdogroller/hotdogroller.vmdl");

        SetupPhysicsFromModel(PhysicsMotionType.Keyframed);

        Tags.Add("interactable");
    }

    /// <summary>
    /// Sets up the entity's UI when the client is spawned 
    /// </summary>
    private void SetupUI()
    {
        Tooltip = new UI.Tooltip(this, "Press E to use machine");
    }
}
