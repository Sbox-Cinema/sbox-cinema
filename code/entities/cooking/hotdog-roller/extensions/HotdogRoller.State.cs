namespace Cinema;

public partial class HotdogRoller
{
    public enum State : int
    {
        BothOff,
        BothOn,
        FrontOn,
        BackOn
    }
    public State MachineState { get; set; }
    /// <summary>
    /// 
    /// </summary>
    private void SetInitState()
    {
        TransitionStateTo(State.BothOff);
    }
    /// <summary>
    ///
    /// </summary>
    private void HandleState()
    {
        switch (MachineState)
        {
            case State.BothOff:
                HandleBothOffState();
                break;
            case State.BothOn:
                HandleBothOnState();
                break;
            case State.BackOn:
                HandleLeftOnState();
                break;
            case State.FrontOn:
                HandleRightOnState();
                break;
        }
    }
    /// <summary>
    ///
    /// </summary>
    private void TransitionStateTo(State state)
    {
        MachineState = state;

        HandleState();
    }
    /// <summary>
    ///
    /// </summary>
    private void HandleBothOffState()
    {
        SetMaterialGroup(0);
    }
    /// <summary>
    ///
    /// </summary>
    private void HandleBothOnState()
    {
        SetMaterialGroup(1);
    }
    /// <summary>
    ///
    /// </summary>
    private void HandleLeftOnState()
    {
        SetMaterialGroup(3);
    }
    /// <summary>
    ///
    /// </summary>
    private void HandleRightOnState()
    {
        SetMaterialGroup(4);
    }
}
