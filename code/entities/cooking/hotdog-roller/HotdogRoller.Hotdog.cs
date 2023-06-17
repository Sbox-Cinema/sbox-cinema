using Sandbox;
using System;

namespace Cinema.HotdogRoller;

public partial class HotdogCookable : ModelEntity
{
    // TODO cooking 
    public bool Reversed { get; set; }
    private Roller rollerParent {get; set;}
    private Particles steam { get; set; }
    private float currentCook = 0;

    public HotdogCookable()
    {
    }

    public HotdogCookable(Roller rollerParent)
    {
        this.rollerParent = rollerParent;
    }

    public override void Spawn()
    {
        base.Spawn();

        Transmit = TransmitType.Always;

        SetModel("models/hotdog/hotdog_roller.vmdl");

        steam = Particles.Create("particles/food_steam/hotdogsteam.vpcf", this);
        steam.SetPosition(0, CollisionWorldSpaceCenter);
        steam.EnableDrawing = false;
    }

    [GameEvent.Tick.Server]
    public void Tick()
    {
        if (rollerParent.Switch.TogglePower != steam.EnableDrawing)
            steam.EnableDrawing = rollerParent.Switch.TogglePower;

        if (!rollerParent.Switch.TogglePower)
            return;

        var parent =

        currentCook += Time.Delta * rollerParent.Knob.KnobRotation;

        switch (currentCook)
        {
            case <= 50:
                SetMaterialGroup(1);
            break;
            case <= 100:
                SetMaterialGroup(2);
            break;
            case > 150:
                SetMaterialGroup(3);
            break;
        }

        LocalRotation = LocalRotation.RotateAroundAxis(Vector3.Forward, Reversed ? 2 : -2);
    }
}
