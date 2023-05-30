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
    private void UpdatePowerState()
    {
        if (!LeftSwitch.IsOn() && !RightSwitch.IsOn())
        {
            TransitionStateTo(State.BothOff);

            return;
        }

        if (LeftSwitch.IsOn() && RightSwitch.IsOn())
        {
            TransitionStateTo(State.BothOn);

            return;
        }

        if (LeftSwitch.IsOn() && !RightSwitch.IsOn())
        {
            TransitionStateTo(State.BackOn);

            return;
        }

        if (!LeftSwitch.IsOn() && RightSwitch.IsOn())
        {
            TransitionStateTo(State.FrontOn);

            return;
        }
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
                HandleBackOnState();
                break;
            case State.FrontOn:
                HandleFrontOnState();
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

        Roller.SetPos(0);
    }
    /// <summary>
    ///
    /// </summary>
    private void HandleBothOnState()
    {
        SetMaterialGroup(1);

        Roller.SetPos(1);         
    }

    /// <summary>
    ///
    /// </summary>
    private void HandleFrontOnState()
    {
        SetMaterialGroup(2);

        Roller.SetPos(2);
    }

    /// <summary>
    ///
    /// </summary>
    private void HandleBackOnState()
    {
        SetMaterialGroup(3);

        Roller.SetPos(3);
    }
}
