using Sandbox;
using System;

namespace Cinema;


public partial class CupFillable : ModelEntity
{
    public Dispenser DispenserParent { get; set; }
    private bool IsAssembled { get; set; }
    public CupFillable() //For the compiler...
    {
       
    }
    public CupFillable(Dispenser dispenserParent)
    {
        DispenserParent = dispenserParent;
        DispenserParent.Cup = this;
    }
    public override void Spawn()
    {
        base.Spawn();

        Transmit = TransmitType.Always;

        SetModel("models/papercup/walker/sodafountaincup/papercup.vmdl");

        SetupPhysicsFromModel(PhysicsMotionType.Static);

        // Make cup a random color on spawn
        Random random = new Random();
        SetMaterialGroup(random.Next(0, 5));
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
    }
    public bool CanPickup()
    {
        return !DispenserParent.IsDispensing && IsAssembled;
    }
    public bool Assembled()
    {
        return IsAssembled;
    }
    public void Assemble()
    {
        //Set to "assembled" body group
        SetBodyGroup(1, 1);

        IsAssembled = true;
    }
}
