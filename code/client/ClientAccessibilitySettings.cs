using Sandbox;

namespace Cinema;

/// <summary>
/// Holds clientside accessibility settings for a player.
/// </summary>
public class ClientAccessibilitySettings : EntityComponent
{
    /// <summary>
    /// The color used by various UI elements to indicate that something is "good".
    /// </summary>
    public string ColorStatusGood { get; set; } = "#5c5cdb";
    /// <summary>
    /// The color used by various UI elements to indicate that something is "bad".
    /// </summary>
    public string ColorStatusBad { get; set; } = "#ad0909";
}
