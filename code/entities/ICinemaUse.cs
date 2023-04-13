using Sandbox;

namespace Cinema;

public interface ICinemaUse : IUse
{
    /// <summary>
    /// Text to show after "Press/Hold {Interact Key} to ...."
    /// In the UI interaction prompt
    /// </summary>
    public string UseText => "Use";

    /// <summary>
    /// Text to show when this entity cannot be used.
    /// `ShowCannotUsePrompt` must be true for this to show.
    /// </summary>
    public string CannotUseText => "Cannot use now";

    /// <summary>
    /// Called when a player stops using this entity
    /// </summary>
    /// <param name="user"></param>
    public void OnStopUse(Entity user);

    /// <summary>
    /// Should we show a "Cannot use" prompt
    /// </summary>
    public bool ShowCannotUsePrompt => false;

    /// <summary>
    /// Does this entity require you to hold the interact key
    /// To complete a timed action
    /// </summary>
    public bool TimedUse => false;

    /// <summary>
    /// Percentage [0, 1] complete of this timed action 
    /// </summary>
    public float TimedUsePercentage => 0;

}
