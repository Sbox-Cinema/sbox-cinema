using Editor;
using Sandbox;
using Cinema;

namespace Cinema.Interactables;

[Library("ent_hotdog_roller"), HammerEntity]
[Title("Hotdog Roller"), Category("Gameplay"), Icon("microwave")]
[EditorModel("models/hotdogroller/hotdogroller.vmdl")]
public partial class HotdogRoller : AnimatedEntity, ICinemaUse
{
    [Net]
    public BaseInteractable Rollers { get; set; }

    [Net]
    public BaseInteractable Knobs { get; set; }
    public BaseInteractable Switches { get; set; }

    /// <summary>
    /// Set up the model when spawned by the server
    /// Setup model
    /// Setup interactions
    /// </summary>
    public override void Spawn()
    {
        base.Spawn();

        Transmit = TransmitType.Always;

        SetModel("models/hotdogroller/hotdogroller.vmdl");

        SetupPhysicsFromModel(PhysicsMotionType.Keyframed);

        Tags.Add("interactable");
    }

    [GameEvent.Tick]
    public void Tick()
    {
        Rollers.Tick();
        Knobs.Tick();
        Switches.Tick();
    }
}
