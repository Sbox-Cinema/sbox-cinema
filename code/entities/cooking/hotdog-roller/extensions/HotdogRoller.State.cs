namespace Cinema;

public partial class HotdogRoller
{
    public enum State : int
    {
        Off,
        On
    }

    public State MachineState { get; set; }
   
    /// <summary>
    ///
    /// </summary>
    private void HandleState()
    {
        switch (MachineState)
        {
            case State.Off:
                HandleOffState();
                break;

            case State.On:
                HandleOnState();
                break;
        }
    }

    /// <summary>
    ///
    /// </summary>
    /*
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
            case State.LeftOn:
                HandleLeftOnState();
                break;
            case State.RightOn:
                HandleRightOnState();
                break;
        }
    }
    */
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
    private void HandleOffState()
    {
        // Power Lights
        SetMaterialGroup(0);

        foreach (var powerSwitch in Components.GetAll<Switch>())
        {
            powerSwitch.SetPos(0);
        }

        foreach (var knob in Components.GetAll<Knob>())
        {
            knob.SetPos(0);
        }

        // Hotdogs
        foreach (var element in Hotdogs)
        {
            var hotdog = element.Value;

            hotdog.Components.RemoveAll();
        }
    }

    /// <summary>
    ///
    /// </summary>
    private void HandleOnState()
    {
        // Power Lights
        SetMaterialGroup(1);

        // Power Switches
        foreach (var powerSwitch in Components.GetAll<Switch>())
        {
            powerSwitch.SetPos(1);
        }

        foreach (var knob in Components.GetAll<Knob>())
        {
            knob.SetPos(3);
        }
        
        // Hotdogs
        foreach (var element in Hotdogs)
        {
            var hotdog = element.Value;

            hotdog.Components.Create<Rotator>();
            hotdog.Components.Create<Cooking>();
        }
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
