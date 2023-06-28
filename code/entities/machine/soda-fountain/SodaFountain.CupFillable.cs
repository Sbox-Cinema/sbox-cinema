using Sandbox;
using System;

namespace Cinema;


public partial class CupFillable : ModelEntity
{
    public CupFillable() //For the compiler...
    {
       
    }
    public override void Spawn()
    {
        base.Spawn();

        Transmit = TransmitType.Always;

        SetModel("models/papercup/sodafountain_cup_01/sodafountain_cup_02.vmdl_c");
        SetupPhysicsFromModel(PhysicsMotionType.Static);

        // Make cup a random color on spawn
        Random random = new Random();
        SetMaterialGroup(random.Next(0, 5));
    }
}
