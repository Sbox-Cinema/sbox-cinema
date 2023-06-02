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
    /// Sets initial state for the Hotdog Roller. Default is both rollers off
    /// </summary>
    private void SetInitState()
    {
        TransitionStateTo(State.BothOff);
    }

    /// <summary>
    /// Updates the power state when one of the power switches changes state
    /// </summary>
    private void UpdatePowerState()
    {
        if (!Switches.IsBackRollerPoweredOn() && !Switches.IsFrontRollerPoweredOn())
        {
            TransitionStateTo(State.BothOff);

            return;
        }

        if (Switches.IsBackRollerPoweredOn() && Switches.IsFrontRollerPoweredOn())
        {
            TransitionStateTo(State.BothOn);

            return;
        }

        if (Switches.IsBackRollerPoweredOn() && !Switches.IsFrontRollerPoweredOn())
        {
            TransitionStateTo(State.BackOn);

            return;
        }

        if (!Switches.IsBackRollerPoweredOn() && Switches.IsFrontRollerPoweredOn())
        {
            TransitionStateTo(State.FrontOn);

            return;
        }
    }

    /// <summary>
    /// Handles state for the hotdog roller
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
    /// Handles transition to new state
    /// </summary>
    private void TransitionStateTo(State state)
    {
        MachineState = state;

        HandleState();
    }
    /// <summary>
    /// Handles state for both rollers off
    /// </summary>
    private void HandleBothOffState()
    {
        SetMaterialGroup(0);

        Roller.SetPos(0);
    }
    /// <summary>
    /// Handles state for both rollers on
    /// </summary>
    private void HandleBothOnState()
    {
        SetMaterialGroup(1);

        Roller.SetPos(1);         
    }

    /// <summary>
    /// Handles state for front roller on
    /// </summary>
    private void HandleFrontOnState()
    {
        SetMaterialGroup(2);

        Roller.SetPos(2);
    }

    /// <summary>
    /// Handles state for back roller on
    /// </summary>
    private void HandleBackOnState()
    {
        SetMaterialGroup(3);

        Roller.SetPos(3);
    }
}
