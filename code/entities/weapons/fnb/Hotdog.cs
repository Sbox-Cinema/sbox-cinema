using Sandbox;

namespace Cinema;

public partial class Hotdog : FoodBase
{
    public override string Name => "Hotdog";
    public override string Description => "As the Germans say: 'Heißer Hund'";
    public override string Icon => "ui/icons/hotdog.png";
    public override Model WorldModel => Model.Load("models/hotdog/w_hotdog_boxed.vmdl");
    public override string ViewModelPath => "models/hotdog/v_hotdog_boxed.vmdl";
    public override float PrimaryFireRate => 1.35f;
    public override int BaseUses => 4;
    public override float NutritionGain => 8;
    public override float HydrationGain => -1;
}
