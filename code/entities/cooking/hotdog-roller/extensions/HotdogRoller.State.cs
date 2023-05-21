using Sandbox;

namespace Cinema;

public partial class HotdogRoller
{
    public enum State : int
    {
        Off,
        On
    }

    [Net] public State MachineState { get; set; } = State.Off;

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
        SetAnimParameter("toggle_left", true);
        SetAnimParameter("toggle_right", true);

        // Control Knobs
        SetAnimParameter("LeftHandleState", 3);
        SetAnimParameter("RightHandleState", 3);

        // Hotdogs
        foreach (var element in Hotdogs)
        {
            var hotdog = element.Value;

            hotdog.Components.Create<Rotator>();
            hotdog.Components.Create<Cooking>();
        }
    }
}
