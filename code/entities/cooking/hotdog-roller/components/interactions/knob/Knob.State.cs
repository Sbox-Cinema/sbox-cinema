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



    /*
    /// <summary>
    ///
    /// </summary>
    private void HandlePosZeroState()
    {
        if (KnobSide == Side.Left)
        {
            Machine.SetAnimParameter("LeftHandleState", 0);
        }

        if (KnobSide == Side.Right)
        {
            Machine.SetAnimParameter("RightHandleState", 0);
        }
    }
    /// <summary>
    ///
    /// </summary>
    private void HandlePosOneState()
    {
        if (KnobSide == Side.Left)
        {
            Machine.SetAnimParameter("LeftHandleState", 1);
        }

        if (KnobSide == Side.Right)
        {
            Machine.SetAnimParameter("RightHandleState", 1);
        }
    }
    /// <summary>
    ///
    /// </summary>
    private void HandlePosTwoState()
    {
        if (KnobSide == Side.Left)
        {
            Machine.SetAnimParameter("LeftHandleState", 2);
        }

        if (KnobSide == Side.Right)
        {
            Machine.SetAnimParameter("RightHandleState", 2);
        }
    }
    /// <summary>
    ///
    /// </summary>
    private void HandlePosThreeState()
    {
        if (KnobSide == Side.Left)
        {
            Machine.SetAnimParameter("LeftHandleState", 3);
        }

        if (KnobSide == Side.Right)
        {
            Machine.SetAnimParameter("RightHandleState", 3);
        }
    }
    /// <summary>
    ///
    /// </summary>
    private void HandlePosFourState()
    {
        if (KnobSide == Side.Left)
        {
            Machine.SetAnimParameter("LeftHandleState", 4);
        }

        if (KnobSide == Side.Right)
        {
            Machine.SetAnimParameter("RightHandleState", 4);
        }
    }
    /// <summary>
    ///
    /// </summary>
    private void HandlePosFiveState()
    {
        if (KnobSide == Side.Left)
        {
            Machine.SetAnimParameter("LeftHandleState", 5);
        }

        if (KnobSide == Side.Right)
        {
            Machine.SetAnimParameter("RightHandleState", 5);
        }
    }
    /// <summary>
    ///
    /// </summary>
    private void HandlePosSixState()
    {
        if (KnobSide == Side.Left)
        {
            Machine.SetAnimParameter("LeftHandleState", 6);
        }

        if (KnobSide == Side.Right)
        {
            Machine.SetAnimParameter("RightHandleState", 6);
        }
    }
    /// <summary>
    ///
    /// </summary>
    private void HandlePosSevenState()
    {
        if (KnobSide == Side.Left)
        {
            Machine.SetAnimParameter("LeftHandleState", 7);
        }

        if (KnobSide == Side.Right)
        {
            Machine.SetAnimParameter("RightHandleState", 7);
        }
    }
    */
}
