namespace Cinema;

public partial class RightSwitch
{
    /// <summary>
    /// Overrides base to handle left switch off state
    /// </summary>
    protected override void HandleOffState()
    {
        Entity.SetAnimParameter("toggle_right", false);
    }
    /// <summary>
    /// Overrides base to handle right switch off state
    /// </summary>
    protected override void HandleOnState()
    {
        Entity.SetAnimParameter("toggle_right", true);
    }
}
