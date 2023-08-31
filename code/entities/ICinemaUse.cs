using Sandbox;

namespace Cinema;

public interface ICinemaUse : IUse
{
    /// <summary>
    /// Text to show after "Press/Hold {Interact Key} to ...."
    /// In the UI interaction prompt
    /// Only used client-side for UI purposes
    /// </summary>
    public string UseText => "Use";

    /// <summary>
    /// Text to show when this entity cannot be used.
    /// `ShowCannotUsePrompt` must be true for this to show.
    /// Only used client-side for UI purposes
    /// </summary>
    public string CannotUseText => "Cannot use now";

    /// <summary>
    /// Called when a player stops using this entity
    /// Returns true if the player can stop using this entity.
    /// Returns false if the player should not be allowed to stop using the entity.
    /// </summary>
    /// <param name="user"></param>
    public bool OnStopUse(Entity user) => true;

    /// <summary>
    /// Should we show a "Cannot use" prompt
    /// Only used client-side for UI purposes
    /// </summary>
    public bool ShowCannotUsePrompt => false;

    /// <summary>
    /// Does this entity require you to hold the interact key to complete a timed action
    /// A user can be prevented from "stopping use" by returning false in OnStopUse
    /// </summary>
    public bool TimedUse => false;

    /// <summary>
    /// Percentage [0, 1] complete of this timed action 
    /// Only used client-side for UI purposes
    /// </summary>
    public float TimedUsePercentage => 0;

    /// <summary>
    /// Is the use of this entity toggled on/off instead of held
    /// A user can be prevented from "stopping use" by returning false in OnStopUse
    /// </summary>
    public bool ToggleUse => false;
}
