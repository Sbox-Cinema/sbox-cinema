namespace Cinema;

public partial class Knob
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

    private State KnobState { get; set; }

    /// <summary>
    ///
    /// </summary>
    private void HandleState()
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
    public void TransitionStateTo(State state)
    {
        KnobState = state;

        HandleState();
    }

    /// <summary>
    ///
    /// </summary>
    private void HandlePosState(State state)
    {
        if (KnobSide == Side.Left)
        {
            Machine.SetAnimParameter("LeftHandleState", (int) state);
        }

        if (KnobSide == Side.Right)
        {
            Machine.SetAnimParameter("RightHandleState", (int) state);
        }
    }
}
