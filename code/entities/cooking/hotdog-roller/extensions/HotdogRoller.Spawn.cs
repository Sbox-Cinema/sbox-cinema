using Sandbox;

namespace Cinema;

public partial class HotdogRoller
{
    public override void Spawn()
    {
        base.Spawn();

        SetupModel();

        TestHotDogs();
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
    /// Sets up the UI when the machine is interacted with
    /// </summary>
    private void SetupUI()
    {
        Tooltip = new UI.Tooltip(this, UseText);
    }

    private void TestHotDogs()
    { 
        for (int i = 0; i < 11; i++)
        {
            HotdogsFront.Add(new Cookable.Hotdog());
            AttachEntity(HotdogsFront[i], $"S{i + 1}F");
        }

        for (int i = 0; i < 11; i++)
        {
            HotdogsBack.Add(new Cookable.Hotdog());
            AttachEntity(HotdogsBack[i], $"S{i + 1}B");
        }
    }

    private void AttachEntity(Entity ent, string attach)
    {
        if (GetAttachment(attach) is Transform t)
        {
            ent.Transform = t;

            ent.Position += (Vector3.Up * 0.5f);
            ent.Position += (Vector3.Left * 0.5f);
        }
    }
}
