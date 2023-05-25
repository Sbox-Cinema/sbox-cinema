namespace Cinema;

public partial class LeftSwitch
{
    /// <summary>
    ///
    /// </summary>
    protected override void HandleOffState()
    {
        Log.Info($"Handling Left Switch Position State {SwitchState}");
        Machine.SetAnimParameter("toggle_left", false);
    }
    /// <summary>
    ///
    /// </summary>
    protected override void HandleOnState()
    {
        Log.Info($"Handling Left Switch Position State {SwitchState}");
        Machine.SetAnimParameter("toggle_left", true);
    }
}
