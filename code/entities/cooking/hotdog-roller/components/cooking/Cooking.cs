using Sandbox;

namespace Cinema;
public partial class Cooking : EntityComponent
{
    private TimeSince SinceCooking { get; set; }

    private float TimeToCook = 8.0f;
    protected override void OnActivate()
    {
        base.OnActivate();

        SetInitState();
    }
    protected override void OnDeactivate()
    {
        base.OnDeactivate();
    }

    private void SetInitState()
    {
        SinceCooking = 0.0f;

        TransitionStateTo(CookState.Raw);
    }

    [GameEvent.Tick]
    private void OnTick()
    {
        if (SinceCooking > TimeToCook && State is CookState.Raw)
        {
            TransitionStateTo(CookState.Cooked);
        } 
    }
}
