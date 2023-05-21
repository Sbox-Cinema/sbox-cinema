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
        for (int i = 0; i < 22; i++)
        {
            Hotdogs.Add($"S{i + 1}F", new Cookable.Hotdog());
            Hotdogs.Add($"S{i + 1}B", new Cookable.Hotdog());
        }

        foreach(var hotdog in Hotdogs)
        {
            AttachEntity(hotdog.Key, hotdog.Value);
        }
    }

    private void AttachEntity(string attach, Entity ent)
    {
        if (GetAttachment(attach) is Transform t)
        {
            ent.Transform = t;

            ent.Position += (Vector3.Up * 0.5f);
            ent.Position += (Vector3.Left * 0.5f);
        }
    }
}
