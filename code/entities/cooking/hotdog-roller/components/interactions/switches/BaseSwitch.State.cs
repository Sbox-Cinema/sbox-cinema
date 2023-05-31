using Sandbox;

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
    /// Handles Switch State
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
    /// Transition to new state
    /// </summary>
    private void TransitionStateTo(State state)
    {
        SwitchState = state;

        HandleState();
    }
    /// <summary>
    /// Handles Off State
    /// </summary>
    protected virtual void HandleOffState()
    {
       
    }
    /// <summary>
    /// Handles On State
    /// </summary>
    protected virtual void HandleOnState()
    {
       
    }
}
