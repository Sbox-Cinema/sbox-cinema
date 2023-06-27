using Sandbox;

namespace Cinema;

public partial class CupFillable : ModelEntity
{
    private Dispenser DispenserParent { get; set; }
    private Particles Soda { get; set; }

    public CupFillable()
    {

    }

    public CupFillable(Dispenser dispenserParent)
    {
        DispenserParent = dispenserParent;
    }

    public override void Spawn()
    {
        base.Spawn();

        Transmit = TransmitType.Always;

        SetModel("models/papercup/sodafountain_cup.vmdl");

        EnableDrawing = true;
    }

    [GameEvent.Tick.Server]
    public void Tick()
    {
        //if(DisperParent.IsDispensing()) {
        //
        //}
    }
}
