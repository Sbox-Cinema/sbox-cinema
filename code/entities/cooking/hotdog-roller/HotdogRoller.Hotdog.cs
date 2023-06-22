using Sandbox;

namespace Cinema.HotdogRoller;

public partial class HotdogCookable : ModelEntity
{
    private enum CookingStates
    {
        Raw = 50,
        Cooked = 100,
        Burnt = 150
    }

    public bool Reversed { get; set; }
    private Roller RollerParent {get; set;}
    private Particles Steam { get; set; }
    private float CurrentCook = 0;
    private int RotationSpeed = 2;

    public HotdogCookable()
    {
    }

    public HotdogCookable(Roller rollerParent)
    {
        RollerParent = rollerParent;
    }

    public override void Spawn()
    {
        base.Spawn();

        Transmit = TransmitType.Always;

        SetModel("models/hotdog/hotdog_roller.vmdl");

        Steam = Particles.Create("particles/food_steam/hotdogsteam.vpcf", this);
        Steam.SetPosition(0, CollisionWorldSpaceCenter);
        Steam.EnableDrawing = false;
    }

    [GameEvent.Tick.Server]
    public void Tick()
    {
        if (RollerParent.Switch.TogglePower != Steam.EnableDrawing && CurrentCook > 30)
            Steam.EnableDrawing = RollerParent.Switch.TogglePower;

        if (!RollerParent.Switch.TogglePower)
            return;

        CurrentCook += Time.Delta * RollerParent.Knob.KnobRotation;

        var newMaterialGroup = 0;

        switch (CurrentCook)
        {
            case <= (int)CookingStates.Raw:
                newMaterialGroup = 1;
            break;
            case <= (int)CookingStates.Cooked:
                newMaterialGroup = 2;
            break;
            case > (int)CookingStates.Burnt:
                newMaterialGroup = 3;
            break;
        }

        if (newMaterialGroup != GetMaterialGroup())
            SetMaterialGroup(newMaterialGroup);


        LocalRotation = LocalRotation.RotateAroundAxis(Vector3.Forward, Reversed ? RotationSpeed : -RotationSpeed);
    }
}
