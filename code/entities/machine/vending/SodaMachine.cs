using Editor;
using Sandbox;
using Sandbox.util;
using System.Collections.Generic;

namespace Cinema;

[Library("ent_soda_machine"), HammerEntity]
[Title("Soda Machine"), Category("Gameplay"), Icon("storefront")]
[EditorModel("models/vendingmachine/vendingmachine.vmdl")]
public partial class SodaMachine : AnimatedEntity, ICinemaUse
{
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

        SetModel("models/vendingmachine/vendingmachine.vmdl");

        SetupPhysicsFromModel(PhysicsMotionType.Keyframed);

        AddInteractables();

        Tags.Add("interactable");
    }

    /// <summary>
    /// Adds the interaction volumes from model doc 
    /// </summary>
    public void AddInteractables()
    {
        Interactables = new Dictionary<string, BaseInteractable>();
    }

    /// <summary>
    /// Simulates the interaction volumes on the server
    /// </summary>
    [GameEvent.Tick.Server]
    public void OnServerTick()
    {
        foreach (var (_, interactable) in Interactables)
        {
            
        }
    }
}
