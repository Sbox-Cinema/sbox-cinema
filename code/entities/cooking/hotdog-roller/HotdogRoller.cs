using Editor;
using Sandbox;
using Sandbox.util;
using System.Collections.Generic;

namespace Cinema.HotdogRoller;

public enum IndicatorLight : int
{
    BothOff,
    BothOn,
    FrontOn,
    BackOn,
}

[Library("ent_hotdog_roller"), HammerEntity]
[Title("Hotdog Roller"), Category("Gameplay"), Icon("microwave")]
[EditorModel("models/hotdogroller/hotdogroller.vmdl")]
public partial class HotdogRoller : AnimatedEntity, ICinemaUse
{
    [Net]
    public Dictionary<string, BaseInteractable> Interactables { get; set; }

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

        AddInteractables();

        Tags.Add("interactable");
    }

    public void HandleUse(Entity ply)
    {
        BaseInteractable found = null;
        float nearest = 999999;

        foreach (var interactableData in Interactables)
        {
            var interactable = interactableData.Value;
            var result = interactable.CanRayTrigger(ply.AimRay);

            if (result.Hit && result.Distance < interactable.MaxDistance && result.Distance < nearest)
            {
                nearest = result.Distance;
                found = interactable;
            }
        }

        if (found != null)
            found.Trigger(ply as Player);
    }

    public void AddInteractables()
    {
        Interactables = new Dictionary<string, BaseInteractable>();
        Interactables.Add("L_Knob", new Knob("LeftHandleState")
        {
            Parent = this,
            Mins = new Vector3(17.4033f, -9.391f, 5.3193f),
            Maxs = new Vector3(15.4033f, -4.391f, 10.3193f)
        });

        Interactables.Add("R_Knob", new Knob("RightHandleState")
        {
            Parent = this,
            Mins = new Vector3(17.4033f, 4.391f, 5.3193f),
            Maxs = new Vector3(15.4033f, 9.391f, 10.3193f)
        });

        Interactables.Add("L_Switch", new Switch("toggle_left")
        {
            Parent = this,
            Mins = new Vector3(17.0505f, -12.0183f, 7.7297f),
            Maxs = new Vector3(15.3539f, -10.0183f, 10.5137f)
        });

        Interactables.Add("R_Switch", new Switch("toggle_right")
        {
            Parent = this,
            Mins = new Vector3(17.0505f, 10.0183f, 7.7297f),
            Maxs = new Vector3(15.3539f, 12.0183f, 10.5137f)
        });

        Interactables.Add("F_Roller", new Roller(Interactables["R_Switch"] as Switch, Interactables["R_Knob"] as Knob, true)
        {
            Parent = this,
            Mins = new Vector3(15.0759f, -12.073f, 12.4461f),
            Maxs = new Vector3(0.6771f, 11.927f, 16.4461f)
        });

        Interactables.Add("B_Roller", new Roller(Interactables["L_Switch"] as Switch, Interactables["L_Knob"] as Knob)
        {
            Parent = this,
            Mins = new Vector3(-0.6439f, -12.073f, 12.4461f),
            Maxs = new Vector3(-15.0427f, 11.927f, 16.4461f)
        });
    }

    [GameEvent.Tick]
    public void Tick()
    {
        Interactables["F_Roller"].Tick();
        Interactables["B_Roller"].Tick();
    }
}
