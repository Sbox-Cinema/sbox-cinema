using Sandbox;

namespace Cinema;

public partial class BaseKnob
{
    public enum State : int
    {
        Zero,
        One,
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven
    }
    public State KnobState { get; set; } 

    /// <summary>
    /// Handles knob state
    /// </summary>
    protected virtual void HandleState()
    {
        switch (KnobState)
        {
            case State.Zero:
                HandlePosState(State.Zero);
                break;
            case State.One:
                HandlePosState(State.One);
                break;
            case State.Two:
                HandlePosState(State.Two);
                break;
            case State.Three:
                HandlePosState(State.Three);
                break;
            case State.Four:
                HandlePosState(State.Four);
                break;
            case State.Five:
                HandlePosState(State.Five);
                break;
            case State.Six:
                HandlePosState(State.Six);
                break;
            case State.Seven:
                HandlePosState(State.Seven);
                break;
        }
    }
    /// <summary>
    /// Transition to new state
    /// </summary>
    protected virtual void TransitionStateTo(State state)
    {
        KnobState = state;

        HandleState();
    }

    /// <summary>
    /// Handles knob position state
    /// </summary>
    protected virtual void HandlePosState(State state)
    {
        
    }
}
