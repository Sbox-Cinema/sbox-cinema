namespace Cinema;

public partial class Roller
{
    public enum State : int
    {
       BothOff,
       BothOn,
       FrontOn,
       BackOn
    }
    public State RollerState { get; set; }

    /// <summary>
    ///
    /// </summary>
    protected virtual void HandleState()
    {
        switch (RollerState)
        {
            case State.BothOff:
                HandleBothOffState();
                break;
            case State.BothOn:
                HandleBothOnState();
                break;
            case State.FrontOn:
                HandleFrontOnState();
                break;
            case State.BackOn:
                HandleBackOnState();
                break;

        }
    }
    /// <summary>
    ///
    /// </summary>
    protected virtual void TransitionStateTo(State state)
    {
        RollerState = state;

        HandleState();
    }

    /// <summary>
    ///
    /// </summary>
    protected virtual void HandleBothOnState()
    {
        foreach (var item in Hotdogs)
        {
            var hotdog = item.Value as Hotdog;

            hotdog.Components.Add(new Rotator());
            hotdog.Components.Add(new Cooking());
        }
    }

    /// <summary>
    ///
    /// </summary>
    protected virtual void HandleBothOffState()
    {
        foreach (var item in Hotdogs)
        {
            var hotdog = item.Value as Hotdog;

            hotdog.Components.RemoveAny<Rotator>();
            hotdog.Components.RemoveAny<Cooking>();
        }
    }

    /// <summary>
    ///
    /// </summary>
    protected virtual void HandleFrontOnState()
    {
        foreach (var item in Hotdogs)
        {
            var hotdog = item.Value as Hotdog;

            hotdog.Components.RemoveAny<Rotator>();
            hotdog.Components.RemoveAny<Cooking>();

            if(item.Key.EndsWith('F'))
            {
                hotdog.Components.Add(new Rotator());
                hotdog.Components.Add(new Cooking());
            }
        }
    }

    /// <summary>
    ///
    /// </summary>
    protected virtual void HandleBackOnState()
    {
        foreach (var item in Hotdogs)
        {
            var hotdog = item.Value as Hotdog;

            hotdog.Components.RemoveAny<Rotator>();
            hotdog.Components.RemoveAny<Cooking>();

            if (item.Key.EndsWith('B'))
            {
                hotdog.Components.Add(new Rotator());
                hotdog.Components.Add(new Cooking());
            }
        }
    }
}
