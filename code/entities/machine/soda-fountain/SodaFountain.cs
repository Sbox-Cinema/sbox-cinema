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
    public enum CupColor : int
    {
        White = 0,
        Blue = 1,
        Green = 2,
        Red = 3,
        Black = 4
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

        SetModel("models/sodafountain/sodafountain_01.vmdl");

        SetupPhysicsFromModel(PhysicsMotionType.Keyframed);

        AddInteractables();
        
        Tags.Add("interactable");
    }

    /// <summary>
    /// Plays soda fountain sound on client spawn
    /// </summary>
    public override void ClientSpawn()
    {
        base.ClientSpawn();

        Sound.FromEntity("machine_hum_02", this).SetVolume(8.0f);
    }

    /// <summary>
    /// Adds the interaction volumes from model doc 
    /// </summary>
    public void AddInteractables()
    {
        Interactables.Add("Platform", new Platform()
        .SetParent(this)
        .InitializeFromInteractionBox("platform")
        );

        Interactables.Add("Dispenser1", CreateDispenser("Lever1State", SodaType.Conk)
        .SetParent(this)
        .InitializeFromInteractionBox("tap_1")
        );

        Interactables.Add("Dispenser2", CreateDispenser("Lever2State", SodaType.MionPisz)
        .SetParent(this)
        .InitializeFromInteractionBox("tap_2")
        );

        Interactables.Add("Dispenser3", CreateDispenser("Lever3State", SodaType.Spooge)
        .SetParent(this)
        .InitializeFromInteractionBox("tap_3")
        );
    }
    /// <summary>
    /// Simulates the interaction volumes
    /// </summary>
    [GameEvent.Tick.Server]
    public void OnServerTick()
    {
        foreach (var (_, interactable) in Interactables)
        {
            interactable.Simulate();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="animation"></param>
    /// <param name="type"></param>
    /// <returns> </returns>
    private Dispenser CreateDispenser(string animation, SodaType type)
    {
        var dispenser = new Dispenser()
        {
            AnimationName = animation,
            SodaType = type
        };
       
        return dispenser;
    }
}
