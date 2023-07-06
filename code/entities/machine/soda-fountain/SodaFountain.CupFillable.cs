using Sandbox;

namespace Cinema;

public partial class CupFillable : ModelEntity
{
    public enum MaterialGroup : int
    {
        Default = 0,
        Blue = 1,
        Green = 2,
        Red = 3,
        Black = 4
    }

    static public string CupItemUniqueId = "soda"; 
    static public string CupItemUniqueIdConk = "soda-conk"; 
    static public string CupItemUniqueIdMionPisz = "soda-mionpisz"; 
    static public string CupItemUniqueIdSpooge = "soda-spooge"; 

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
    }
    public void SetCupColor(SodaFountain.SodaType type)
    {
        switch(type)
        {
            case SodaFountain.SodaType.Conk:
                SetMaterialGroup((int)MaterialGroup.Red);
                break;
            case SodaFountain.SodaType.MionPisz:
                SetMaterialGroup((int)MaterialGroup.Blue);
                break;
            case SodaFountain.SodaType.Spooge:
                SetMaterialGroup((int)MaterialGroup.Green);
                break;
        }
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

        // Play sounds for placing the lid on the cup
        Sound.FromEntity("cup_lid_place", Parent);

        IsAssembled = true;
    }
}
