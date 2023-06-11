using Sandbox;
using System;

namespace Cinema;

public partial class Hydration : BaseNeed, ISingletonComponent
{
    public override string DisplayName => "Thirst";
    public override string IconPath => "local_drink";
    public override float SatisfactionPercent
    {
        get
        {
            var hydrationAmount = HydrationLevel / MaxHydrationLevel;
            if (hydrationAmount > HydrationFillThreshold)
            {
                return 100;
            }
            else
            {
                return hydrationAmount.Remap(0, HydrationFillThreshold, 0, 100);
            }
        }
    }

    [Net]
    public float MaxHydrationLevel { get; set; } = 10;
    [Net]
    public float HydrationLevel { get; set; } = 10;
    [Net]
    public float HydrationDecayPerSecond { get; set; } = 1.6f / 60f;
    [Net]
    public float HydrationFillThreshold { get; set; } = 0.8f;


    [GameEvent.Tick.Server]
    public void OnServerTick()
    {
        if (HydrationLevel > 0)
        {
            OffsetHydration(-HydrationDecayPerSecond * Time.Delta * NeedDecayFactor);
        }
        else
        { 
            // TODO: Something bad happens when you dehydrate.
        }
    }

    public void OffsetHydration(float amount)
    {
        HydrationLevel += amount;
        // If we drink to the fill threshold, we should be fully hydrated.
        if (amount > 0 && HydrationLevel / MaxHydrationLevel >= HydrationFillThreshold)
        {
            HydrationLevel = MaxHydrationLevel;
        }
        HydrationLevel = Math.Clamp(HydrationLevel, 0, MaxHydrationLevel);
    }
}
