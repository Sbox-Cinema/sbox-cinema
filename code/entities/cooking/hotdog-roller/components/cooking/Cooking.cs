using Sandbox;

namespace Cinema;
public partial class Cooking : EntityComponent
{
    private float TimeToCook = 8.0f;
    private TimeSince SinceCooking { get; set; }
    private bool DoneCooking => SinceCooking > TimeToCook;
    private bool IsCooked { get; set; } = false;

    /// <summary>
    /// Called when activated
    /// </summary>
    protected override void OnActivate()
    {
        base.OnActivate();

        SinceCooking = 0.0f;
    }

    /// <summary>
    /// 
    /// </summary>
    [GameEvent.Tick]
    private void OnTick()
    {
        if (DoneCooking)
        {
            if (!IsCooked)
            {
                Entity.Components.GetOrCreate<Steam>();

                IsCooked = true;
            }
        } 
    }
}
