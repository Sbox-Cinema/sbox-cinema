using Editor;
using Sandbox;

namespace Cinema;

[Library("ent_hotdog_roller"), HammerEntity]
[Title("Hotdog Roller"), Category("Gameplay"), Icon("microwave")]
[EditorModel("models/hotdogroller/hotdogroller.vmdl")]
public partial class HotdogRoller : AnimatedEntity, ICinemaUse
{
    [BindComponent] public LeftKnob LeftKnob { get; }
    [BindComponent] public RightKnob RightKnob { get; }
    [BindComponent] public LeftSwitch LeftSwitch { get; }
    [BindComponent] public RightSwitch RightSwitch { get; }
    [BindComponent] public Roller Roller { get; }

    /// <summary>
    /// Sets up the model when spawned by the server
    /// Sets model
    /// Sets initial state
    /// </summary>
    public override void Spawn()
    {
        base.Spawn();

        SetupModel();

        SetInitState();
    }
    /// <summary>
    /// Sets up the model when spawned by the server
    /// Sets clientside UI
    /// </summary>
    public override void ClientSpawn()
    {
        base.ClientSpawn();

        SetupUI();
    }
    /// <summary>
    /// Sets up the model when spawned by the server
    /// Sets attached entity components
    /// Adds tags
    /// </summary>
    private void SetupModel()
    {
        Transmit = TransmitType.Always;

        SetModel("models/hotdogroller/hotdogroller.vmdl");

        SetupPhysicsFromModel(PhysicsMotionType.Keyframed);

        Components.Create<LeftKnob>();
        Components.Create<RightKnob>();

        Components.Create<LeftSwitch>();
        Components.Create<RightSwitch>();

        Components.Create<Roller>();
        
        Tags.Add("interactable");
    }
}
