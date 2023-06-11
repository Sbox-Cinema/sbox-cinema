using Sandbox;
using System;

namespace Cinema;

public partial class Nutrition : BaseNeed, ISingletonComponent
{
    public override string DisplayName => "Hunger";
    public override string IconPath => "lunch_dining";
    public override float SatisfactionPercent
    {
        get
        {
            var nutritionAmount = NutritionLevel / MaxNutritionLevel;
            if (nutritionAmount > NutritionFillThreshold)
            {
                return 100;
            }
            else
            {
                return nutritionAmount.Remap(0, NutritionFillThreshold, 0, 100);
            }
        }
    }
    [Net]
    public float MaxNutritionLevel { get; set; } = 10;
    [Net]
    public float NutritionLevel { get; set; } = 10;
    [Net]
    public float NutritionDecayPerSecond { get; set; } = 0.4f / 60f;
    [Net]
    public float NutritionFillThreshold { get; set; } = 0.8f;

    [GameEvent.Tick.Server]
    public void OnServerTick()
    {
        if (NutritionLevel > 0)
        {
            OffsetNutrition(-NutritionDecayPerSecond * Time.Delta * NeedDecayFactor);
        }
        else
        {
            // TODO: Something bad happens when you starve.
        }
    }

    public void OffsetNutrition(float amount)
    {
        NutritionLevel += amount;
        // If we eat to the fill threshold, we should be fully fed.
        if (amount > 0 && NutritionLevel / MaxNutritionLevel >= NutritionFillThreshold)
        {
            NutritionLevel = MaxNutritionLevel;
        }
        NutritionLevel = Math.Clamp(NutritionLevel, 0, MaxNutritionLevel);
    }
}
