using Sandbox;

namespace Cinema;

public partial class Nachos : FoodBase
{
    public override string Name => "Box of Nachos";
    public override string Description => "Its nacho business";
    public override string Icon => "ui/icons/nachos.png";
    public override Model WorldModel => Model.Load("models/nachos_tray/w_nachos_tray.vmdl");
    public override string ViewModelPath => "models/nachos_tray/v_nachos_tray.vmdl";
    public override float PrimaryFireRate => 0.65f;
    public override int BaseUses => 26;
    public override float NutritionGain => 10;
    public override float HydrationGain => -2;
}
