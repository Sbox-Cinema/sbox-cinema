namespace Cinema;

public partial class Cooking
{
    public enum CookState : int
    {
        Raw,
        Cooked
    }
    private CookState State { get; set; }

    /// <summary>
    /// Handles state for cooking
    /// </summary>
    private void HandleState()
    {
        switch (State)
        {
            case CookState.Raw:
                break;

            case CookState.Cooked:
                Entity.Components.GetOrCreate<Steam>();
                break;
        }
    }

    /// <summary>
    /// Handles transition to new state
    /// </summary>
    private void TransitionStateTo(CookState state)
    {
        State = state;

        HandleState();
    }
}
