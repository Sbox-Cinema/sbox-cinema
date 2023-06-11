using Sandbox;

namespace Cinema;

public partial class Popcorn : FoodBase
{
    public override string Name => "Bucket of Popcorn";
    public override string Description => "Tasty popcorn delights";
    public override string Icon => "ui/icons/popcorn.png";
    public override Model WorldModel => Model.Load("models/popcorn_tub/w_popcorn_tub_01.vmdl");
    public override string ViewModelPath => "models/popcorn_tub/v_popcorn_tub_01.vmdl";
    public override float PrimaryFireRate => 0.85f;
    public override int BaseUses => 10;
    public override float NutritionGain => 12;
    public override float HydrationGain => -3;
}
