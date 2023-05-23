namespace Cinema;

public partial class Cooking
{
    public enum CookState : int
    {
        Raw,
        Cooked
    }
    private CookState State { get; set; }

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
    private void TransitionStateTo(CookState state)
    {
        State = state;

        HandleState();
    }
}
