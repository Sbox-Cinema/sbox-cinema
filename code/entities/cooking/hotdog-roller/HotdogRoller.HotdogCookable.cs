using Sandbox;

namespace Cinema.HotdogRoller;

public partial class HotdogCookable : ModelEntity
{
    private enum CookingTime
    {
        Raw = 50,
        Cooked = 100,
        Burnt = 150
    }

    private enum MaterialGroup
    {
        Raw = 1,
        Cooked = 2,
        Burnt = 3
    }
    private int RotationSpeed = 2;
    private int StartSteam = 30;
    public bool Reversed { get; set; }
    private Roller RollerParent {get; set;}
    private Particles Steam { get; set; }
    private float CurrentCook = 0;

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
        if (RollerParent.Switch.TogglePower != Steam.EnableDrawing && CurrentCook > StartSteam)
            Steam.EnableDrawing = RollerParent.Switch.TogglePower;

        if (!RollerParent.Switch.TogglePower)
            return;

        CurrentCook += Time.Delta * RollerParent.Knob.KnobRotation;

        MaterialGroup newMaterialGroup = MaterialGroup.Raw;

        switch (CurrentCook)
        {
            case < (int)CookingTime.Raw: break;
            case < (int)CookingTime.Cooked:
                newMaterialGroup = MaterialGroup.Cooked;
            break;
            case > (int)CookingTime.Burnt:
                newMaterialGroup = MaterialGroup.Burnt;
            break;
        }

        if ((int)newMaterialGroup != GetMaterialGroup() && newMaterialGroup > MaterialGroup.Raw)
            SetMaterialGroup((int)newMaterialGroup);

        LocalRotation = LocalRotation.RotateAroundAxis(Vector3.Forward, Reversed ? RotationSpeed : -RotationSpeed);
    }
}
