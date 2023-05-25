namespace Cinema;

public partial class RightSwitch
{
    /// <summary>
    ///
    /// </summary>
    protected override void HandleOffState()
    {
        Log.Info($"Handling Right Switch Position State {SwitchState}");
        Machine.SetAnimParameter("toggle_right", false);
    }
    /// <summary>
    ///
    /// </summary>
    protected override void HandleOnState()
    {
        Log.Info($"Handling Right Switch Position State {SwitchState}");
        Machine.SetAnimParameter("toggle_right", true);
    }
}
