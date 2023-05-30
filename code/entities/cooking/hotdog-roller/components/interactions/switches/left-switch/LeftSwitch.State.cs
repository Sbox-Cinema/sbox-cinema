namespace Cinema;

public partial class LeftSwitch
{
    /// <summary>
    ///
    /// </summary>
    protected override void HandleOffState()
    {
        Entity.SetAnimParameter("toggle_left", false);
    }
    /// <summary>
    ///
    /// </summary>
    protected override void HandleOnState()
    {
        Entity.SetAnimParameter("toggle_left", true);
    }
}
