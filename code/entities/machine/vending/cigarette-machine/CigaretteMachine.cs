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
    public enum CigarettePackType : int
    {
        StrikeForce = 0,
        SandPort = 1
    }
    [Net] public IList<BaseInteractable> Interactables { get; set; }
    
    /// <summary>
    /// Set up the model when spawned by the server
    /// Setup model
    /// Setup interactions
    /// </summary>
    public override void Spawn()
    {
        base.Spawn();

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
        Interactables.Add(CreatePurchaseButton(CigarettePackType.SandPort)
            .SetParent(this)
            .InitializeFromInteractionBox("prod_cigarette_1")
            );

        Interactables.Add(CreatePurchaseButton(CigarettePackType.StrikeForce)
           .SetParent(this)
           .InitializeFromInteractionBox("prod_cigarette_2")
           );

        Interactables.Add(CreatePurchaseButton(CigarettePackType.StrikeForce)
           .SetParent(this)
           .InitializeFromInteractionBox("prod_cigarette_3")
           );

        Interactables.Add(CreatePurchaseButton(CigarettePackType.StrikeForce)
           .SetParent(this)
           .InitializeFromInteractionBox("prod_cigarette_4")
           );

        Interactables.Add(CreatePurchaseButton(CigarettePackType.StrikeForce)
            .SetParent(this)
            .InitializeFromInteractionBox("prod_cigarette_5")
            );

        Interactables.Add(CreatePurchaseButton(CigarettePackType.StrikeForce)    
           .SetParent(this)
           .InitializeFromInteractionBox("prod_cigarette_6")
           );

        Interactables.Add(CreatePurchaseButton(CigarettePackType.StrikeForce)
           .SetParent(this)
           .InitializeFromInteractionBox("prod_cigarette_7")
           );

        Interactables.Add(CreatePurchaseButton(CigarettePackType.StrikeForce)
           .SetParent(this)
           .InitializeFromInteractionBox("prod_cigarette_8")
           );
    }


    /// <summary>
    /// 
    /// </summary>

    /// <param name="type"></param>
    /// <returns> </returns>
    private PurchaseButton CreatePurchaseButton(CigarettePackType type)
    {
        var purchaseButton = new PurchaseButton()
        {
            CigarettePackType = type
        };

        return purchaseButton;
    }

    /// <summary>
    /// Simulates the interaction volumes
    /// </summary>
    [GameEvent.Tick.Server]
    public void OnServerTick()
    {
        foreach (var interactable in Interactables)
        {
            interactable.Simulate();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <returns> </returns>
    public static string GetCigaretteItemIdByCigarettePackTypType(CigarettePackType type)
    {
        return type switch
        {
            CigarettePackType.StrikeForce => "cigarettepack-strikeforce",
            CigarettePackType.SandPort => "cigarettepack-sandport",
            
            _ => "cigarettepack-strikeforce"
        };
    }
}
