using System.Collections.Generic;
using Editor;
using Sandbox;
using Sandbox.util;

namespace Cinema;

[Library("ent_soda_fountain"), HammerEntity]
[Title("Soda Fountain"), Category("Gameplay"), Icon("coffee_maker")]
[EditorModel("models/sodafountain/sodafountain_01.vmdl")]
public partial class SodaFountain : AnimatedEntity, ICinemaUse
{
    public enum SodaType : int
    {
        Conk = 0,
        MionPisz = 1,
        Spooge = 2
    }

    [Net] public IDictionary<string, BaseInteractable> Interactables { get; set; }

    /// <summary>
    /// Set up the model when spawned by the server
    /// Setup model
    /// Setup interactions
    /// </summary>
    public override void Spawn()
    {
        base.Spawn();

        Transmit = TransmitType.Always;

        SetModel("models/sodafountain/sodafountain_01.vmdl");

        SetupPhysicsFromModel(PhysicsMotionType.Keyframed);

        AddInteractables();
        
        Tags.Add("interactable");
    }

    public void AddInteractables()
    {
        Interactables.Add("Dispenser1", new Dispenser("Lever1State", SodaType.Conk)
        .SetParent(this)
        .SetBoundsFromInteractionBox("tap_1")
        );

        Interactables.Add("Dispenser2", new Dispenser("Lever2State", SodaType.MionPisz)
        .SetParent(this)
        .SetBoundsFromInteractionBox("tap_2")
        );

        Interactables.Add("Dispenser3", new Dispenser("Lever3State", SodaType.Spooge)
        .SetParent(this)
        .SetBoundsFromInteractionBox("tap_3")
        );

        Interactables.Add("Platform", new Platform()
        .SetParent(this)
        .SetBoundsFromInteractionBox("platform")
        );
    }

    [GameEvent.Tick.Server]
    public void OnServerTick()
    {
        foreach (var (_, interactable) in Interactables)
        {
            interactable.Simulate();
        }
    }
}
