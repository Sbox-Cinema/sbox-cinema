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

        Interactables.Add("coin_slot", new CoinSlot()
        //.SetParent(this)
        //.SetBoundsFromInteractionBox("platform")
        );

        Interactables.Add("refund_button", new RefundButton()
        //.SetParent(this)
        //.SetBoundsFromInteractionBox("tap_1")
        );

        Interactables.Add("soda_button_1", new SodaButton()
        //.SetParent(this)
        //.SetBoundsFromInteractionBox("tap_2")
        );

        Interactables.Add("soda_button_2", new SodaButton()
        //.SetParent(this)
        //.SetBoundsFromInteractionBox("tap_2")
        );

        Interactables.Add("soda_button_3", new SodaButton()
        //.SetParent(this)
        //.SetBoundsFromInteractionBox("tap_2")
        );

        Interactables.Add("soda_button_4", new SodaButton()
        //.SetParent(this)
        //.SetBoundsFromInteractionBox("tap_2")
        );

        Interactables.Add("soda_button_5", new SodaButton()
        //.SetParent(this)
        //.SetBoundsFromInteractionBox("tap_2")
        );
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
