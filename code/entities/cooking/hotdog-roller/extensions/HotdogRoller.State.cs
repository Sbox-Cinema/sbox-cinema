using Sandbox;

namespace Cinema;

public partial class HotdogRoller
{
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

        // Power Switches
        SetAnimParameter("toggle_left", false);
        SetAnimParameter("toggle_right", false);

        // Control Knobs
        SetAnimParameter("LeftHandleState", 0);
        SetAnimParameter("RightHandleState", 0);

        // Hotdogs
        foreach (var hotdog in HotdogsFront)
        {
            hotdog.Components.RemoveAll();
        }

        foreach (var hotdog in HotdogsBack)
        {
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
        SetAnimParameter("toggle_left", true);
        SetAnimParameter("toggle_right", true);

        // Control Knobs
        SetAnimParameter("LeftHandleState", 3);
        SetAnimParameter("RightHandleState", 3);

        // Hotdogs
        foreach (var hotdog in HotdogsFront)
        {
            hotdog.Components.Create<Rotator>();
            hotdog.Components.Create<Cooking>();
        }

        foreach (var hotdog in HotdogsBack)
        {
            hotdog.Components.Create<Rotator>();
            hotdog.Components.Create<Cooking>();
        }
    }
}
