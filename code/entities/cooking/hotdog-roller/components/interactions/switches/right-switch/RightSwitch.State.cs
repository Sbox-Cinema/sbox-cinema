namespace Cinema;

public partial class RightSwitch
{
    /// <summary>
    ///
    /// </summary>
    protected override void HandleOffState()
    {
        Entity.SetAnimParameter("toggle_right", false);
    }
    /// <summary>
    ///
    /// </summary>
    protected override void HandleOnState()
    {
        Entity.SetAnimParameter("toggle_right", true);
    }
}
