using Editor;
using Sandbox;
using Sandbox.util;
using System.Collections.Generic;

namespace Cinema;

[Library("ent_cigarette_machine"), HammerEntity]
[Title("Cigarette Machine"), Category("Gameplay"), Icon("smoking_rooms")]
[EditorModel("materials/models/cigarettemachine/cigarettemachine.vmdl")]
public partial class CigaretteMachine : AnimatedEntity, ICinemaUse
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

        SetModel("materials/models/cigarettemachine/cigarettemachine.vmdl");

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
            interactable.Simulate();
        }
    }
}
