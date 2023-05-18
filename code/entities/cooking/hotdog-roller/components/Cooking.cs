using Cinema.Cookable;
using Sandbox;

namespace Cinema;

public partial class Cooking : EntityComponent
{
    private TimeSince SinceCooking { get; set; }

    private float TimeToCook = 15.0f;

    private bool IsSteaming = false;
    protected override void OnActivate()
    {
        base.OnActivate();

        SinceCooking = 0.0f;
    }

    protected override void OnDeactivate()
    {
        base.OnDeactivate();

        Log.Info($"{SinceCooking}");

        SinceCooking = 0.0f;

        Entity.Components.RemoveAny<Steam>();
    }

    [GameEvent.Tick]
    private void OnTick()
    {
        if(SinceCooking > TimeToCook && !IsSteaming)
        {
            Entity.Components.Create<Steam>();

            IsSteaming = true;
        }
    }
}
