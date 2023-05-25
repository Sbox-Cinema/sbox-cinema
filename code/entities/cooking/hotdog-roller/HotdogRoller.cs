using Editor;
using Sandbox;
using System.Collections.Generic;

namespace Cinema;

[Library("ent_hotdog_roller"), HammerEntity]
[Title("Hotdog Roller"), Category("Gameplay"), Icon("microwave")]
[EditorModel("models/hotdogroller/hotdogroller.vmdl")]
public partial class HotdogRoller : AnimatedEntity, ICinemaUse
{
    [Net] public IDictionary<string, Hotdog> Hotdogs {get; set;}
    [BindComponent] public LeftKnob LeftKnob { get; }
    [BindComponent] public LeftKnob RightKnob { get; }
    [BindComponent] public LeftSwitch LeftSwitch { get; }
    [BindComponent] public RightSwitch RightSwitch { get; }
    public override void Spawn()
    {
        base.Spawn();

        SetupModel();

        SetInitState();
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

        Components.GetOrCreate<LeftKnob>();
        Components.GetOrCreate<RightKnob>();

        Components.GetOrCreate<LeftSwitch>();
        Components.GetOrCreate<RightSwitch>();

        Tags.Add("interactable");
    }
}
