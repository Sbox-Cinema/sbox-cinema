using Sandbox;

namespace Cinema.HotdogRoller;

public partial class HotdogCookable : ModelEntity
{
    // TODO cooking 
    public bool Reversed { get; set; }
    private Switch rollerSwitch {get; set;}
    private Particles steam { get; set; }
    private int cookTime = 15;
    private float currentCook = 0;

    public HotdogCookable()
    {
    }

    public HotdogCookable(Switch rollerSwitch)
    {
        this.rollerSwitch = rollerSwitch;
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
        if (rollerSwitch.TogglePower != steam.EnableDrawing)
            steam.EnableDrawing = rollerSwitch.TogglePower;

        if (!rollerSwitch.TogglePower)
            return;

        LocalRotation = LocalRotation.RotateAroundAxis(Vector3.Forward, Reversed ? -2 : 2);
    }
}
