namespace Cinema;

public partial class LeftSwitch
{
    /// <summary>
    /// Overrides base to handle left switch off state
    /// </summary>
    protected override void HandleOffState()
    {
        Entity.SetAnimParameter("toggle_left", false);
    }
    /// <summary>
    /// Overrides base to handle right switch on state
    /// </summary>
    protected override void HandleOnState()
    {
        Entity.SetAnimParameter("toggle_left", true);
    }
}
