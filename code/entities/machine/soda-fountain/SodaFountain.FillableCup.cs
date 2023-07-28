using Sandbox;

namespace Cinema;

public partial class FillableCup : ModelEntity
{
    public Dispenser Dispenser { get; set; }
    public string ItemId { get; set; } 
    public bool IsAssembled { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public override void Spawn()
    {
        base.Spawn();

        SetModel("models/papercup/walker/sodafountaincup/papercup.vmdl");

        SetupPhysicsFromModel(PhysicsMotionType.Static);
    }
    /// <summary>
    /// 
    /// </summary>
    public void Initialize()
    {
        SetMaterialGroup((int)SodaFountain.GetCupColorBySodaType(Dispenser.SodaType));

        Dispenser.SetCup(this);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns> </returns>
    public void Assemble()
    {
        //Set to "assembled" body group
        SetBodyGroup(1, 1);

        // Play sounds for placing the lid on the cup
        Sound.FromEntity("cup_lid_place", Parent);

        IsAssembled = true;
    }
}
