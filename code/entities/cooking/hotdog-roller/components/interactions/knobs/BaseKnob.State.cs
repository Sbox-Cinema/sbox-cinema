using Sandbox;

namespace Cinema;

public partial class BaseKnob
{
    public enum State : int
    {
        PosZero,
        PosOne,
        PosTwo,
        PosThree,
        PosFour,
        PosFive,
        PosSix,
        PosSeven
    }
    public State KnobState { get; set; } 

    /// <summary>
    ///
    /// </summary>
    protected virtual void HandleState()
    {
        switch (KnobState)
        {
            case State.PosZero:
                HandlePosState(State.PosZero);
                break;
            case State.PosOne:
                HandlePosState(State.PosOne);
                break;
            case State.PosTwo:
                HandlePosState(State.PosTwo);
                break;
            case State.PosThree:
                HandlePosState(State.PosThree);
                break;
            case State.PosFour:
                HandlePosState(State.PosFour);
                break;
            case State.PosFive:
                HandlePosState(State.PosFive);
                break;
            case State.PosSix:
                HandlePosState(State.PosSix);
                break;
            case State.PosSeven:
                HandlePosState(State.PosSeven);
                break;
        }
    }
    /// <summary>
    ///
    /// </summary>
    protected virtual void TransitionStateTo(State state)
    {
        KnobState = state;

        HandleState();
    }

    /// <summary>
    ///
    /// </summary>
    protected virtual void HandlePosState(State state)
    {
        
    }
}
