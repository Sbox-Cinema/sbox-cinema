namespace Cinema;

public partial class Switch
{
    public enum State : int
    {
        Off,
        On
    }
    
    public State SwitchState { get; set; }
    
    /// <summary>
    ///
    /// </summary>
    private void HandleState()
    {
        switch (SwitchState)
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
    private void TransitionStateTo(State state)
    {
        SwitchState = state;

        HandleState();
    }
    /// <summary>
    ///
    /// </summary>
    private void HandleOffState()
    {
        if (SwitchSide == Side.Left)
        {
            Machine.SetAnimParameter("toggle_left", false);
        }

        if (SwitchSide == Side.Right)
        {
            Machine.SetAnimParameter("toggle_right", false);
        }
    }
    /// <summary>
    ///
    /// </summary>
    private void HandleOnState()
    {
        if (SwitchSide == Side.Left)
        {
            Machine.SetAnimParameter("toggle_left", true);
        }

        if (SwitchSide == Side.Right)
        {
            Machine.SetAnimParameter("toggle_right", true);
        }
    }
}
