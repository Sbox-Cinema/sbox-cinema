using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema;

public class Stomach : EntityComponent<Player>
{
    protected Nutrition Nutrition 
    { 
        get
        {
            _Nutrition ??= Entity.Components.GetOrCreate<Nutrition>();
            return _Nutrition;
        }
    }
    private Nutrition _Nutrition;

    protected Hydration Hydration
    {
        get
        {
            _Hydration ??= Entity.Components.GetOrCreate<Hydration>();
            return _Hydration;
        }
    }
    private Hydration _Hydration;

    public void Ingest(FoodBase foodItem)
    {
        if (foodItem == null) 
            return;

        Log.Info($"Ingesting food item {foodItem.Name}, nutrition {foodItem.NutritionPerUse}, hydration: {foodItem.HydrationPerUse}");
        Nutrition.OffsetNutrition(foodItem.NutritionPerUse);
        Hydration.OffsetHydration(foodItem.HydrationPerUse);
    }
}
