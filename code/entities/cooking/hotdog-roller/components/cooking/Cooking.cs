using Sandbox;

namespace Cinema;
public partial class Cooking : EntityComponent
{
    private TimeSince SinceCooking { get; set; }

    private float TimeToCook = 8.0f;

    /// <summary>
    /// Called when activated
    /// Sets initial state
    /// </summary>
    protected override void OnActivate()
    {
        base.OnActivate();

        SetInitState();
    }

    /// <summary>
    /// Sets initial state default (Raw)
    /// </summary>
    private void SetInitState()
    {
        SinceCooking = 0.0f;

        TransitionStateTo(CookState.Raw);
    }

    /// <summary>
    /// Checks for raw to cooked state transition
    /// </summary>
    [GameEvent.Tick]
    private void OnTick()
    {
        if (SinceCooking > TimeToCook && State is CookState.Raw)
        {
            TransitionStateTo(CookState.Cooked);
        } 
    }
}
