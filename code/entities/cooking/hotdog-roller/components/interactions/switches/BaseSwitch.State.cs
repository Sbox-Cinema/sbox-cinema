namespace Cinema;

public partial class BaseSwitch
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
    protected virtual void HandleOffState()
    {
       
    }
    /// <summary>
    ///
    /// </summary>
    protected virtual void HandleOnState()
    {
       
    }
}
