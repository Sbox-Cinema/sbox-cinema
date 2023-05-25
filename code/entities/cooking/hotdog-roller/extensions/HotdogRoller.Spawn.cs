using Sandbox;

namespace Cinema;

public partial class HotdogRoller
{
    private int MaxRollerItems = 22;
    public override void Spawn()
    {
        base.Spawn();

        SetupModel();

        SetInitState();

        //TestHotDogs();
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

        Components.Add(new Knob(Knob.Side.Left));
        Components.Add(new Knob(Knob.Side.Right));

        Components.Add(new Switch(Switch.Side.Left));
        Components.Add(new Switch(Switch.Side.Right));

        Tags.Add("interactable");
    }

    private void TestHotDogs()
    { 
        for (int i = 0; i < MaxRollerItems; i++)
        {
            Hotdogs.Add($"S{i + 1}F", new Hotdog());
            Hotdogs.Add($"S{i + 1}B", new Hotdog());
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
