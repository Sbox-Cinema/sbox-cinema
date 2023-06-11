using Sandbox;

namespace Cinema;

public partial class Soda : FoodBase
{
    public override string Name => "Cup of Soda";
    public override string Description => "A delicious taste of soda";
    public override string Icon => "ui/icons/soda.png";
    public override Model WorldModel => Model.Load("models/papercup/w_papercup.vmdl");
    public override string ViewModelPath => "models/papercup/v_papercup.vmdl";
    public override float PrimaryFireRate => 1.35f;
    public override int BaseUses => 8;
    public override string EatSound => "drink_slurp";
    public override float NutritionGain => 0;
    public override float HydrationGain => 15;
}
